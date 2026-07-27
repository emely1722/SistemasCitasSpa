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

namespace SistemasCitasSpa.Controllers
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var dato = dto.UsuarioOCorreo.Trim();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.NombreUsuario == dato ||
                    u.Correo == dato);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseña incorrectos"
                });
            }

            if (!usuario.Activo)
            {
                return Unauthorized(new
                {
                    mensaje = "El usuario está inactivo"
                });
            }

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.ClaveHash,
                dto.Clave);

            if (resultado == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    mensaje = "Usuario o contraseña incorrectos"
                });
            }

            var token = GenerarToken(usuario);

            int duracion = int.TryParse(
                _configuration["Jwt:DurationInMinutes"],
                out int minutos)
                ? minutos
                : 60;

            return Ok(new
            {
                token,
                expiracion = DateTime.UtcNow.AddMinutes(duracion),
                usuario = usuario.NombreUsuario
            });
        }

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
                    usuario.Correo)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credenciales = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            int duracion = int.TryParse(
                _configuration["Jwt:DurationInMinutes"],
                out int minutos)
                ? minutos
                : 60;

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(duracion),
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}