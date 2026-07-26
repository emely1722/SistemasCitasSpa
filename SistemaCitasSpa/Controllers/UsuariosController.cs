using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
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

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsuarios()
        {
            return await _context.Usuarios
                .Select(u => new
                {
                    u.IdUsuario,
                    u.NombreUsuario,
                    u.NombreCompleto,
                    u.Correo,
                    u.Rol
                })
                .ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.IdUsuario == id)
                .Select(u => new
                {
                    u.IdUsuario,
                    u.NombreUsuario,
                    u.NombreCompleto,
                    u.Correo,
                    u.Rol
                })
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound(new { mensaje = $"No se encontró el usuario con ID {id}." });
            }

            return usuario;
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(RegistroDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == dto.NombreUsuario))
            {
                return BadRequest(new { mensaje = "El nombre de usuario ya está registrado." });
            }

            var usuario = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                NombreCompleto = dto.NombreUsuario, // Puedes ajustar esto si agregas NombreCompleto a tu DTO
                Correo = dto.Email,
                ClaveHash = HashPassword(dto.Password),
                Rol = "Usuario"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, new
            {
                usuario.IdUsuario,
                usuario.NombreUsuario,
                usuario.NombreCompleto,
                usuario.Correo,
                usuario.Rol
            });
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, ActualizarUsuarioDto dto)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null)
            {
                return NotFound(new { mensaje = $"No existe un usuario con el ID {id}." });
            }

            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == dto.NombreUsuario && u.IdUsuario != id))
            {
                return BadRequest(new { mensaje = "El nombre de usuario ya está ocupado por otro registro." });
            }

            usuarioExistente.NombreUsuario = dto.NombreUsuario;
            usuarioExistente.Correo = dto.Email;
            usuarioExistente.Rol = dto.Rol;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                usuarioExistente.ClaveHash = HashPassword(dto.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound(new { mensaje = "El usuario ya no existe." });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { mensaje = "Usuario actualizado correctamente." });
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = $"No se encontró el usuario con el ID {id} para eliminar." });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Usuario con ID {id} eliminado exitosamente." });
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}