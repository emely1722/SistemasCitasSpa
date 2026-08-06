using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemasCitasSpa.Data;
using SistemasCitasSpa.DTOs;
using SistemasCitasSpa.Models;

namespace SistemasCitasSpa.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuariosController(
            AppDbContext context,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: api/Usuario
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .Select(u => new
                {
                    u.IdUsuario,
                    u.NombreCompleto,
                    u.NombreUsuario,
                    u.Correo,
                    u.Activo
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // GET: api/Usuario/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.IdUsuario == id)
                .Select(u => new
                {
                    u.IdUsuario,
                    u.NombreCompleto,
                    u.NombreUsuario,
                    u.Correo,
                    u.Activo
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensaje = "El usuario no existe"
                });
            }

            return Ok(usuario);
        }

        // POST: api/Usuario
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostUsuario(
            RegistroUsuarioDto dto)
        {
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == dto.NombreUsuario);

            if (usuarioExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El nombre de usuario ya está registrado"
                });
            }

            var correoExiste = await _context.Usuarios
                .AnyAsync(u => u.Correo == dto.Correo);

            if (correoExiste)
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

            return CreatedAtAction(
                nameof(GetUsuario),
                new { id = usuario.IdUsuario },
                new
                {
                    usuario.IdUsuario,
                    usuario.NombreCompleto,
                    usuario.NombreUsuario,
                    usuario.Correo,
                    usuario.Activo
                });
        }

        // PUT: api/Usuario/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(
            int id,
            ActualizarUsuarioDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensaje = "El usuario no existe"
                });
            }

            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u =>
                    u.NombreUsuario == dto.NombreUsuario &&
                    u.IdUsuario != id);

            if (usuarioExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El nombre de usuario ya está registrado"
                });
            }

            var correoExiste = await _context.Usuarios
                .AnyAsync(u =>
                    u.Correo == dto.Correo &&
                    u.IdUsuario != id);

            if (correoExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El correo ya está registrado"
                });
            }

            usuario.NombreCompleto = dto.NombreCompleto;
            usuario.NombreUsuario = dto.NombreUsuario;
            usuario.Correo = dto.Correo;
            usuario.Activo = dto.Activo;

            if (!string.IsNullOrWhiteSpace(dto.Clave))
            {
                usuario.ClaveHash = _passwordHasher.HashPassword(
                    usuario,
                    dto.Clave);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Usuario actualizado correctamente"
            });
        }

        // DELETE: api/Usuario
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensaje = "El usuario no existe"
                });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}