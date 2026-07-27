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
    public class HorariosTerapeutasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HorariosTerapeutasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/HorariosTerapeutas
        [HttpGet]
        public async Task<IActionResult> GetHorarios()
        {
            var horarios = await _context.HorariosTerapeutas
                .AsNoTracking()
                .Select(h => new
                {
                    h.IdHorario,
                    h.IdTerapeuta,
                    Terapeuta = h.Terapeuta != null
                        ? h.Terapeuta.Nombre + " " + h.Terapeuta.Apellido
                        : null,
                    h.DiaSemana,
                    h.HoraInicio,
                    h.HoraFin,
                    h.Activo
                })
                .ToListAsync();

            return Ok(horarios);
        }

        // GET: api/HorariosTerapeutas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHorario(int id)
        {
            var horario = await _context.HorariosTerapeutas
                .AsNoTracking()
                .Where(h => h.IdHorario == id)
                .Select(h => new
                {
                    h.IdHorario,
                    h.IdTerapeuta,
                    Terapeuta = h.Terapeuta != null
                        ? h.Terapeuta.Nombre + " " + h.Terapeuta.Apellido
                        : null,
                    h.DiaSemana,
                    h.HoraInicio,
                    h.HoraFin,
                    h.Activo
                })
                .FirstOrDefaultAsync();

            if (horario == null)
            {
                return NotFound(new
                {
                    mensaje = "El horario no existe"
                });
            }

            return Ok(horario);
        }

        // POST: api/HorariosTerapeutas
        [HttpPost]
        public async Task<IActionResult> PostHorario(
            HorarioTerapeutaDto dto)
        {
            var terapeutaExiste = await _context.Terapeutas
                .AnyAsync(t => t.IdTerapeuta == dto.IdTerapeuta);

            if (!terapeutaExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El terapeuta no existe"
                });
            }

            if (dto.HoraFin <= dto.HoraInicio)
            {
                return BadRequest(new
                {
                    mensaje = "La hora final debe ser mayor que la hora inicial"
                });
            }

            var horario = new HorarioTerapeuta
            {
                IdTerapeuta = dto.IdTerapeuta,
                DiaSemana = dto.DiaSemana,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                Activo = dto.Activo
            };

            _context.HorariosTerapeutas.Add(horario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetHorario),
                new { id = horario.IdHorario },
                horario);
        }

        // PUT: api/HorariosTerapeutas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHorario(
            int id,
            HorarioTerapeutaDto dto)
        {
            var horario = await _context.HorariosTerapeutas.FindAsync(id);

            if (horario == null)
            {
                return NotFound(new
                {
                    mensaje = "El horario no existe"
                });
            }

            var terapeutaExiste = await _context.Terapeutas
                .AnyAsync(t => t.IdTerapeuta == dto.IdTerapeuta);

            if (!terapeutaExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El terapeuta no existe"
                });
            }

            if (dto.HoraFin <= dto.HoraInicio)
            {
                return BadRequest(new
                {
                    mensaje = "La hora final debe ser mayor que la hora inicial"
                });
            }

            horario.IdTerapeuta = dto.IdTerapeuta;
            horario.DiaSemana = dto.DiaSemana;
            horario.HoraInicio = dto.HoraInicio;
            horario.HoraFin = dto.HoraFin;
            horario.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Horario actualizado correctamente"
            });
        }

        // DELETE: api/HorariosTerapeutas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHorario(int id)
        {
            var horario = await _context.HorariosTerapeutas.FindAsync(id);

            if (horario == null)
            {
                return NotFound(new
                {
                    mensaje = "El horario no existe"
                });
            }

            _context.HorariosTerapeutas.Remove(horario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}