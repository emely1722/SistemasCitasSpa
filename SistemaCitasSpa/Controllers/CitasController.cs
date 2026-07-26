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
    public class CitasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CitasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Citas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cita>>> GetCitas()
        {
            return await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Include(c => c.Terapeuta)
                .Include(c => c.Sala)
                .Include(c => c.MetodoPago)
                .ToListAsync();
        }

        // GET: api/Citas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cita>> GetCita(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Include(c => c.Terapeuta)
                .Include(c => c.Sala)
                .Include(c => c.MetodoPago)
                .FirstOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
            {
                return NotFound(new { mensaje = $"No se encontró la cita con ID {id}." });
            }

            return cita;
        }

        // POST: api/Citas
        [HttpPost]
        public async Task<ActionResult<Cita>> PostCita(CrearCitaDto dto)
        {
            DateTime fechaHoraCita = dto.Fecha.ToDateTime(dto.Hora);
            if (fechaHoraCita < DateTime.Now)
            {
                return BadRequest(new { mensaje = "No se pueden agendar citas en fechas o horas pasadas." });
            }

            var cliente = await _context.Clientes.FindAsync(dto.IdCliente);
            if (cliente == null)
                return BadRequest(new { mensaje = "El cliente especificado no existe." });

            var servicio = await _context.Servicios.FindAsync(dto.IdServicio);
            if (servicio == null)
                return BadRequest(new { mensaje = "El servicio especificado no existe." });

            var terapeuta = await _context.Terapeutas.FindAsync(dto.IdTerapeuta);
            if (terapeuta == null)
                return BadRequest(new { mensaje = "El terapeuta especificado no existe." });

            var sala = await _context.Salas.FindAsync(dto.IdSala);
            if (sala == null)
                return BadRequest(new { mensaje = "La sala especificada no existe." });

            var cita = new Cita
            {
                IdCliente = dto.IdCliente,
                Fecha = dto.Fecha,
                Hora = dto.Hora,
                IdServicio = dto.IdServicio,
                DuracionMinutos = servicio.DuracionMinutos,
                IdTerapeuta = dto.IdTerapeuta,
                IdSala = dto.IdSala,
                IdMetodoPago = dto.IdMetodoPago,
                FechaRegistro = DateTime.Now
            };

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCita), new { id = cita.IdCita }, cita);
        }

        // PUT: api/Citas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCita(int id, Cita cita)
        {
            if (id != cita.IdCita)
            {
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID de la cita enviada." });
            }

            var citaExistente = await _context.Citas.FindAsync(id);
            if (citaExistente == null)
            {
                return NotFound(new { mensaje = $"No existe una cita registrada con el ID {id} para actualizar." });
            }

            DateTime fechaHoraCita = cita.Fecha.ToDateTime(cita.Hora);
            if (fechaHoraCita < DateTime.Now)
            {
                return BadRequest(new { mensaje = "No se puede actualizar una cita a una fecha o hora pasada." });
            }

            citaExistente.IdCliente = cita.IdCliente;
            citaExistente.Fecha = cita.Fecha;
            citaExistente.Hora = cita.Hora;
            citaExistente.IdServicio = cita.IdServicio;
            citaExistente.DuracionMinutos = cita.DuracionMinutos;
            citaExistente.IdTerapeuta = cita.IdTerapeuta;
            citaExistente.IdSala = cita.IdSala;
            citaExistente.IdMetodoPago = cita.IdMetodoPago;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitaExists(id))
                {
                    return NotFound(new { mensaje = "La cita dejó de existir durante la actualización." });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { mensaje = "Cita actualizada correctamente.", cita = citaExistente });
        }

        // DELETE: api/Citas/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound(new { mensaje = $"No se puede eliminar. No existe la cita con el ID {id}." });
            }

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Cita con ID {id} eliminada exitosamente." });
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.IdCita == id);
        }
    }
}