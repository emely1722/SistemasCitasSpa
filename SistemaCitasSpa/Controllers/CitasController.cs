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

        // GET: api/Clientes
        [HttpGet]
        public async Task<IActionResult> GetCitas()
        {
            var citas = await _context.Citas
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Include(c => c.Terapeuta)
                .Include(c => c.Sala)
                .Include(c => c.MetodoPago)
                .ToListAsync();

            var resultado = citas.Select(c => MostrarCita(c));

            return Ok(resultado);
        }

        // GET: api/Clientes
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCita(int id)
        {
            var cita = await _context.Citas
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Include(c => c.Servicio)
                .Include(c => c.Terapeuta)
                .Include(c => c.Sala)
                .Include(c => c.MetodoPago)
                .FirstOrDefaultAsync(c => c.IdCita == id);

            if (cita == null)
            {
                return NotFound(new
                {
                    mensaje = "La cita no existe"
                });
            }

            return Ok(MostrarCita(cita));
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<IActionResult> PostCita(CitaDto dto)
        {
            var fechaHora = dto.Fecha.Date.Add(dto.Hora);

            if (fechaHora <= DateTime.Now)
            {
                return BadRequest(new
                {
                    mensaje = "No se puede registrar una cita en una fecha pasada"
                });
            }

            var cliente = await _context.Clientes.FindAsync(dto.IdCliente);

            if (cliente == null)
                return BadRequest(new { mensaje = "El cliente no existe" });

            var servicio = await _context.Servicios.FindAsync(dto.IdServicio);

            if (servicio == null)
                return BadRequest(new { mensaje = "El servicio no existe" });

            var terapeuta = await _context.Terapeutas.FindAsync(dto.IdTerapeuta);

            if (terapeuta == null)
                return BadRequest(new { mensaje = "El terapeuta no existe" });

            var sala = await _context.Salas.FindAsync(dto.IdSala);

            if (sala == null)
                return BadRequest(new { mensaje = "La sala no existe" });

            if (dto.IdMetodoPago.HasValue)
            {
                var metodo = await _context.MetodosPago
                    .FindAsync(dto.IdMetodoPago.Value);

                if (metodo == null)
                    return BadRequest(new { mensaje = "El método de pago no existe" });
            }

            var cita = new Cita
            {
                IdCliente = dto.IdCliente,
                Fecha = dto.Fecha.Date,
                Hora = dto.Hora,
                IdServicio = dto.IdServicio,
                DuracionMinutos = servicio.DuracionMinutos,
                IdTerapeuta = dto.IdTerapeuta,
                IdSala = dto.IdSala,
                IdMetodoPago = dto.IdMetodoPago
            };

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCita),
                new { id = cita.IdCita },
                new
                {
                    mensaje = "Cita registrada correctamente",
                    cita.IdCita
                });
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCita(
            int id,
            CitaDto dto)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound(new
                {
                    mensaje = "La cita no existe"
                });
            }

            var fechaHora = dto.Fecha.Date.Add(dto.Hora);

            if (fechaHora <= DateTime.Now)
            {
                return BadRequest(new
                {
                    mensaje = "La fecha y hora no pueden estar en el pasado"
                });
            }

            if (!await _context.Clientes
                .AnyAsync(c => c.IdCliente == dto.IdCliente))
            {
                return BadRequest(new { mensaje = "El cliente no existe" });
            }

            var servicio = await _context.Servicios.FindAsync(dto.IdServicio);

            if (servicio == null)
            {
                return BadRequest(new { mensaje = "El servicio no existe" });
            }

            if (!await _context.Terapeutas
                .AnyAsync(t => t.IdTerapeuta == dto.IdTerapeuta))
            {
                return BadRequest(new { mensaje = "El terapeuta no existe" });
            }

            if (!await _context.Salas
                .AnyAsync(s => s.IdSala == dto.IdSala))
            {
                return BadRequest(new { mensaje = "La sala no existe" });
            }

            if (dto.IdMetodoPago.HasValue &&
                !await _context.MetodosPago.AnyAsync(m =>
                    m.IdMetodoPago == dto.IdMetodoPago.Value))
            {
                return BadRequest(new
                {
                    mensaje = "El método de pago no existe"
                });
            }

            cita.IdCliente = dto.IdCliente;
            cita.Fecha = dto.Fecha.Date;
            cita.Hora = dto.Hora;
            cita.IdServicio = dto.IdServicio;
            cita.DuracionMinutos = servicio.DuracionMinutos;
            cita.IdTerapeuta = dto.IdTerapeuta;
            cita.IdSala = dto.IdSala;
            cita.IdMetodoPago = dto.IdMetodoPago;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Cita actualizada correctamente"
            });
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCita(int id)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound(new
                {
                    mensaje = "La cita no existe"
                });
            }

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private object MostrarCita(Cita cita)
        {
            var inicio = cita.Fecha.Date.Add(cita.Hora);
            var final = inicio.AddMinutes(cita.DuracionMinutos);
            var ahora = DateTime.Now;

            string estado;

            if (ahora < inicio)
                estado = "Vigente";
            else if (ahora < final)
                estado = "En proceso";
            else
                estado = "Finalizado";

            var restante = inicio - ahora;

            int dias = 0;
            int horas = 0;

            if (restante > TimeSpan.Zero)
            {
                dias = restante.Days;
                horas = restante.Hours;
            }

            return new
            {
                cita.IdCita,

                Cliente = cita.Cliente == null
                    ? null
                    : cita.Cliente.Nombre + " " + cita.Cliente.Apellido,

                cita.Fecha,
                cita.Hora,

                Servicio = cita.Servicio?.Nombre,

                cita.DuracionMinutos,

                Terapeuta = cita.Terapeuta == null
                    ? null
                    : cita.Terapeuta.Nombre + " " + cita.Terapeuta.Apellido,

                Sala = cita.Sala?.Nombre,

                MetodoPago = cita.MetodoPago?.Nombre,

                DiasRestantes = dias,
                HorasRestantes = horas,
                Estado = estado
            };
        }
    }
}