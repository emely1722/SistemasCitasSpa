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

        // GET: api/Citas/5
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

        // POST: api/Citas
        [HttpPost]
        public async Task<IActionResult> PostCita(CitaDto dto)
        {
            var fecha = dto.Fecha.Date;
            var inicio = fecha.Add(dto.Hora);

            // No permitir fechas u horas pasadas
            if (inicio <= DateTime.Now)
            {
                return BadRequest(new
                {
                    mensaje = "No se puede registrar una cita en una fecha u hora pasada"
                });
            }

            // Validar cliente
            var cliente = await _context.Clientes
                .FindAsync(dto.IdCliente);

            if (cliente == null)
            {
                return BadRequest(new
                {
                    mensaje = "El cliente no existe"
                });
            }

            // Validar servicio
            var servicio = await _context.Servicios
                .FindAsync(dto.IdServicio);

            if (servicio == null)
            {
                return BadRequest(new
                {
                    mensaje = "El servicio no existe"
                });
            }

            // Validar terapeuta
            var terapeuta = await _context.Terapeutas
                .FindAsync(dto.IdTerapeuta);

            if (terapeuta == null)
            {
                return BadRequest(new
                {
                    mensaje = "El terapeuta no existe"
                });
            }

            // Validar sala
            var sala = await _context.Salas
                .FindAsync(dto.IdSala);

            if (sala == null)
            {
                return BadRequest(new
                {
                    mensaje = "La sala no existe"
                });
            }

            // Validar método de pago si fue enviado
            if (dto.IdMetodoPago.HasValue)
            {
                var metodoPago = await _context.MetodosPago
                    .FindAsync(dto.IdMetodoPago.Value);

                if (metodoPago == null)
                {
                    return BadRequest(new
                    {
                        mensaje = "El método de pago no existe"
                    });
                }
            }

            // La duración se obtiene automáticamente del servicio
            var fin = inicio.AddMinutes(servicio.DuracionMinutos);

            // Validar horario del terapeuta
            var dia = ObtenerDiaSemana(inicio.DayOfWeek);

            var horarios = await _context.HorariosTerapeutas
                .AsNoTracking()
                .Where(h =>
                    h.IdTerapeuta == dto.IdTerapeuta &&
                    h.DiaSemana == dia &&
                    h.Activo)
                .ToListAsync();

            var horarioValido = horarios.Any(h =>
                dto.Hora >= h.HoraInicio &&
                fin.TimeOfDay <= h.HoraFin &&
                fin.Date == inicio.Date);

            if (!horarioValido)
            {
                return BadRequest(new
                {
                    mensaje = "La cita está fuera del horario disponible del terapeuta"
                });
            }

            var fechaSiguiente = fecha.AddDays(1);

            // Validar cruce de horario del terapeuta
            var citasTerapeuta = await _context.Citas
                .AsNoTracking()
                .Where(c =>
                    c.IdTerapeuta == dto.IdTerapeuta &&
                    c.Fecha >= fecha &&
                    c.Fecha < fechaSiguiente)
                .ToListAsync();

            var terapeutaOcupado = citasTerapeuta.Any(c =>
            {
                var inicioCita = c.Fecha.Date.Add(c.Hora);
                var finCita = inicioCita.AddMinutes(c.DuracionMinutos);

                return inicio < finCita && fin > inicioCita;
            });

            if (terapeutaOcupado)
            {
                return Conflict(new
                {
                    mensaje = "El terapeuta ya tiene una cita en ese horario"
                });
            }

            // Validar cruce de horario de la sala
            var citasSala = await _context.Citas
                .AsNoTracking()
                .Where(c =>
                    c.IdSala == dto.IdSala &&
                    c.Fecha >= fecha &&
                    c.Fecha < fechaSiguiente)
                .ToListAsync();

            var salaOcupada = citasSala.Any(c =>
            {
                var inicioCita = c.Fecha.Date.Add(c.Hora);
                var finCita = inicioCita.AddMinutes(c.DuracionMinutos);

                return inicio < finCita && fin > inicioCita;
            });

            if (salaOcupada)
            {
                return Conflict(new
                {
                    mensaje = "La sala ya está ocupada en ese horario"
                });
            }

            var cita = new Cita
            {
                IdCliente = dto.IdCliente,
                Fecha = fecha,
                Hora = dto.Hora,
                IdServicio = dto.IdServicio,

                // Duración automática
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

        // PUT: api/Citas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCita(int id, CitaDto dto)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound(new
                {
                    mensaje = "La cita no existe"
                });
            }

            var fecha = dto.Fecha.Date;
            var inicio = fecha.Add(dto.Hora);

            if (inicio <= DateTime.Now)
            {
                return BadRequest(new
                {
                    mensaje = "La fecha y hora de la cita no pueden estar en el pasado"
                });
            }

            // Validar cliente
            var clienteExiste = await _context.Clientes
                .AnyAsync(c => c.IdCliente == dto.IdCliente);

            if (!clienteExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El cliente no existe"
                });
            }

            // Validar servicio
            var servicio = await _context.Servicios
                .FindAsync(dto.IdServicio);

            if (servicio == null)
            {
                return BadRequest(new
                {
                    mensaje = "El servicio no existe"
                });
            }

            // Validar terapeuta
            var terapeutaExiste = await _context.Terapeutas
                .AnyAsync(t => t.IdTerapeuta == dto.IdTerapeuta);

            if (!terapeutaExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El terapeuta no existe"
                });
            }

            // Validar sala
            var salaExiste = await _context.Salas
                .AnyAsync(s => s.IdSala == dto.IdSala);

            if (!salaExiste)
            {
                return BadRequest(new
                {
                    mensaje = "La sala no existe"
                });
            }

            // Validar método de pago
            if (dto.IdMetodoPago.HasValue)
            {
                var metodoExiste = await _context.MetodosPago
                    .AnyAsync(m =>
                        m.IdMetodoPago == dto.IdMetodoPago.Value);

                if (!metodoExiste)
                {
                    return BadRequest(new
                    {
                        mensaje = "El método de pago no existe"
                    });
                }
            }

            var fin = inicio.AddMinutes(servicio.DuracionMinutos);

            // Validar horario del terapeuta
            var dia = ObtenerDiaSemana(inicio.DayOfWeek);

            var horarios = await _context.HorariosTerapeutas
                .AsNoTracking()
                .Where(h =>
                    h.IdTerapeuta == dto.IdTerapeuta &&
                    h.DiaSemana == dia &&
                    h.Activo)
                .ToListAsync();

            var horarioValido = horarios.Any(h =>
                dto.Hora >= h.HoraInicio &&
                fin.TimeOfDay <= h.HoraFin &&
                fin.Date == inicio.Date);

            if (!horarioValido)
            {
                return BadRequest(new
                {
                    mensaje = "La cita está fuera del horario disponible del terapeuta"
                });
            }

            var fechaSiguiente = fecha.AddDays(1);

            // Validar cruce del terapeuta
            var citasTerapeuta = await _context.Citas
                .AsNoTracking()
                .Where(c =>
                    c.IdCita != id &&
                    c.IdTerapeuta == dto.IdTerapeuta &&
                    c.Fecha >= fecha &&
                    c.Fecha < fechaSiguiente)
                .ToListAsync();

            var terapeutaOcupado = citasTerapeuta.Any(c =>
            {
                var inicioCita = c.Fecha.Date.Add(c.Hora);
                var finCita = inicioCita.AddMinutes(c.DuracionMinutos);

                return inicio < finCita && fin > inicioCita;
            });

            if (terapeutaOcupado)
            {
                return Conflict(new
                {
                    mensaje = "El terapeuta ya tiene una cita en ese horario"
                });
            }

            // Validar cruce de la sala
            var citasSala = await _context.Citas
                .AsNoTracking()
                .Where(c =>
                    c.IdCita != id &&
                    c.IdSala == dto.IdSala &&
                    c.Fecha >= fecha &&
                    c.Fecha < fechaSiguiente)
                .ToListAsync();

            var salaOcupada = citasSala.Any(c =>
            {
                var inicioCita = c.Fecha.Date.Add(c.Hora);
                var finCita = inicioCita.AddMinutes(c.DuracionMinutos);

                return inicio < finCita && fin > inicioCita;
            });

            if (salaOcupada)
            {
                return Conflict(new
                {
                    mensaje = "La sala ya está ocupada en ese horario"
                });
            }

            cita.IdCliente = dto.IdCliente;
            cita.Fecha = fecha;
            cita.Hora = dto.Hora;
            cita.IdServicio = dto.IdServicio;

            // Se vuelve a obtener la duración del servicio
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

        // DELETE: api/Citas/5
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
            var fin = inicio.AddMinutes(cita.DuracionMinutos);
            var ahora = DateTime.Now;

            string estado;

            if (ahora < inicio)
            {
                estado = "Vigente";
            }
            else if (ahora < fin)
            {
                estado = "En proceso";
            }
            else
            {
                estado = "Finalizado";
            }

            var tiempoRestante = inicio - ahora;

            int diasRestantes = 0;
            int horasRestantes = 0;

            if (tiempoRestante > TimeSpan.Zero)
            {
                diasRestantes = tiempoRestante.Days;
                horasRestantes = tiempoRestante.Hours;
            }

            return new
            {
                cita.IdCita,

                cita.IdCliente,

                Cliente = cita.Cliente == null
                    ? null
                    : cita.Cliente.Nombre + " " + cita.Cliente.Apellido,

                cita.Fecha,
                cita.Hora,

                cita.IdServicio,

                Servicio = cita.Servicio?.Nombre,

                cita.DuracionMinutos,

                cita.IdTerapeuta,

                Terapeuta = cita.Terapeuta == null
                    ? null
                    : cita.Terapeuta.Nombre + " " + cita.Terapeuta.Apellido,

                cita.IdSala,

                Sala = cita.Sala?.Nombre,

                cita.IdMetodoPago,

                MetodoPago = cita.MetodoPago?.Nombre,

                DiasRestantes = diasRestantes,
                HorasRestantes = horasRestantes,
                Estado = estado
            };
        }

        private string ObtenerDiaSemana(DayOfWeek dia)
        {
            return dia switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => ""
            };
        }
    }
}