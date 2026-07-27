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
    public class SalasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Salas
        [HttpGet]
        public async Task<IActionResult> GetSalas()
        {
            var salas = await _context.Salas
                .AsNoTracking()
                .ToListAsync();

            return Ok(salas);
        }

        // GET: api/Salas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSala(int id)
        {
            var sala = await _context.Salas
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.IdSala == id);

            if (sala == null)
            {
                return NotFound(new
                {
                    mensaje = "La sala no existe"
                });
            }

            return Ok(sala);
        }

        // POST: api/Salas
        [HttpPost]
        public async Task<IActionResult> PostSala(SalaDto dto)
        {
            var existe = await _context.Salas
                .AnyAsync(s => s.Nombre == dto.Nombre);

            if (existe)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe una sala con ese nombre"
                });
            }

            var sala = new Sala
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activa = dto.Activa
            };

            _context.Salas.Add(sala);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSala),
                new { id = sala.IdSala },
                sala);
        }

        // PUT: api/Salas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSala(int id, SalaDto dto)
        {
            var sala = await _context.Salas.FindAsync(id);

            if (sala == null)
            {
                return NotFound(new
                {
                    mensaje = "La sala no existe"
                });
            }

            var existe = await _context.Salas
                .AnyAsync(s =>
                    s.Nombre == dto.Nombre &&
                    s.IdSala != id);

            if (existe)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe otra sala con ese nombre"
                });
            }

            sala.Nombre = dto.Nombre;
            sala.Descripcion = dto.Descripcion;
            sala.Activa = dto.Activa;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Sala actualizada correctamente"
            });
        }

        // DELETE: api/Salas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSala(int id)
        {
            var sala = await _context.Salas.FindAsync(id);

            if (sala == null)
            {
                return NotFound(new
                {
                    mensaje = "La sala no existe"
                });
            }

            var tieneCitas = await _context.Citas
                .AnyAsync(c => c.IdSala == id);

            if (tieneCitas)
            {
                return BadRequest(new
                {
                    mensaje = "No se puede eliminar la sala porque tiene citas registradas"
                });
            }

            _context.Salas.Remove(sala);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}