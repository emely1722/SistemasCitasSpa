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
    public class TerapeutasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TerapeutasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Terapeutas
        [HttpGet]
        public async Task<IActionResult> GetTerapeutas()
        {
            var terapeutas = await _context.Terapeutas
                .AsNoTracking()
                .ToListAsync();

            return Ok(terapeutas);
        }

        // GET: api/Terapeutas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTerapeuta(int id)
        {
            var terapeuta = await _context.Terapeutas
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IdTerapeuta == id);

            if (terapeuta == null)
            {
                return NotFound(new
                {
                    mensaje = "El terapeuta no existe"
                });
            }

            return Ok(terapeuta);
        }

        // POST: api/Terapeutas
        [HttpPost]
        public async Task<IActionResult> PostTerapeuta(TerapeutaDto dto)
        {
            var terapeuta = new Terapeuta
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Telefono = dto.Telefono,
                Correo = dto.Correo,
                Especialidad = dto.Especialidad,
                Activo = dto.Activo
            };

            _context.Terapeutas.Add(terapeuta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTerapeuta),
                new { id = terapeuta.IdTerapeuta },
                terapeuta);
        }

        // PUT: api/Terapeutas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTerapeuta(
            int id,
            TerapeutaDto dto)
        {
            var terapeuta = await _context.Terapeutas.FindAsync(id);

            if (terapeuta == null)
            {
                return NotFound(new
                {
                    mensaje = "El terapeuta no existe"
                });
            }

            terapeuta.Nombre = dto.Nombre;
            terapeuta.Apellido = dto.Apellido;
            terapeuta.Telefono = dto.Telefono;
            terapeuta.Correo = dto.Correo;
            terapeuta.Especialidad = dto.Especialidad;
            terapeuta.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Terapeuta actualizado correctamente"
            });
        }

        // DELETE: api/Terapeutas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTerapeuta(int id)
        {
            var terapeuta = await _context.Terapeutas.FindAsync(id);

            if (terapeuta == null)
            {
                return NotFound(new
                {
                    mensaje = "El terapeuta no existe"
                });
            }

            var tieneCitas = await _context.Citas
                .AnyAsync(c => c.IdTerapeuta == id);

            var tieneHorarios = await _context.HorariosTerapeutas
                .AnyAsync(h => h.IdTerapeuta == id);

            if (tieneCitas || tieneHorarios)
            {
                return BadRequest(new
                {
                    mensaje = "No se puede eliminar porque el terapeuta tiene citas u horarios registrados"
                });
            }

            _context.Terapeutas.Remove(terapeuta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}