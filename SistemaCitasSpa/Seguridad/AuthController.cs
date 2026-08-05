using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemasCitasSpa.Data;
using SistemasCitasSpa.DTOs;
using SistemasCitasSpa.Models;

namespace SistemaCitasSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthController(
            AppDbContext context,
            IConfiguration configuration,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        // POST: api/Auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistroUsuarioDto dto)
        {
            var existeUsuario = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == dto.NombreUsuario);

            if (existeUsuario)
            {
                return BadRequest(new
                {
                    mensaje = "El nombre de usuario ya está registrado"
                });
            }

            var existeCorreo = await _context.Usuarios
                .AnyAsync(u => u.Correo == dto.Correo);

            if (existeCorreo)
            {
                return BadRequest(new
                {
                    mensaje = "El correo ya está registrado"
                });
            }

            var usuario = new Usuario
            {
                NombreCompleto = dto.NombreCompleto,
                NombreUsuario = dto.NombreUsuario,
                Correo = dto.Correo,
                ClaveHash = "",
                Activo = true
            };

            usuario.ClaveHash = _passwordHasher.HashPassword(
                usuario,
                dto.Clave);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                mensaje = "Usuario registrado correctamente",
                usuario.IdUsuario,
                usuario.NombreCompleto,
                usuario.NombreUsuario,
                usuario.Correo
            });
        }

        // POST: api/Auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            Console.WriteLine("");
            Console.WriteLine("Dato recibido: " + dto.UsuarioOCorreo);
            Console.WriteLine("Clave recibida: " + dto.Clave);

            var dato = dto.UsuarioOCorreo.Trim();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.NombreUsuario == dato ||
                    u.Correo == dato);

            Console.WriteLine("Usuario encontrado: " + (usuario != null));

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseña incorrectos"
                });
            }

            Console.WriteLine("Nombre Usuario: " + usuario.NombreUsuario);
            Console.WriteLine("Correo: " + usuario.Correo);
            Console.WriteLine("Hash BD: " + usuario.ClaveHash);

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.ClaveHash,
                dto.Clave);

            Console.WriteLine("Resultado PasswordHasher: " + resultado);

            if (resultado == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseña incorrectos"
                });
            }

            var token = GenerarToken(usuario);

            return Ok(new
            {
                token
            });
        }

        //token
        private string GenerarToken(Usuario usuario)
        {
            var claims = new[]
            {
        new Claim(
            ClaimTypes.NameIdentifier,
            usuario.IdUsuario.ToString()),

        new Claim(
            ClaimTypes.Name,
            usuario.NombreUsuario),

        new Claim(
            ClaimTypes.Email,
            usuario.Correo),

        new Claim(
            JwtRegisteredClaimNames.Jti,
            Guid.NewGuid().ToString())
    };

            var clave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credenciales = new SigningCredentials(
                clave,
                SecurityAlgorithms.HmacSha256);

            var duracion = Convert.ToDouble(
                _configuration["Jwt:DurationInMinutes"]);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(duracion),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}