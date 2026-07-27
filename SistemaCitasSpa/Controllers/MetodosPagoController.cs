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
    public class MetodosPagoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MetodosPagoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/MetodosPago
        [HttpGet]
        public async Task<IActionResult> GetMetodosPago()
        {
            var metodos = await _context.MetodosPago
                .AsNoTracking()
                .ToListAsync();

            return Ok(metodos);
        }

        // GET: api/MetodosPago/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMetodoPago(int id)
        {
            var metodo = await _context.MetodosPago
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdMetodoPago == id);

            if (metodo == null)
            {
                return NotFound(new
                {
                    mensaje = "El método de pago no existe"
                });
            }

            return Ok(metodo);
        }

        // POST: api/MetodosPago
        [HttpPost]
        public async Task<IActionResult> PostMetodoPago(MetodoPagoDto dto)
        {
            var existe = await _context.MetodosPago
                .AnyAsync(m => m.Nombre == dto.Nombre);

            if (existe)
            {
                return BadRequest(new
                {
                    mensaje = "Ya existe un método de pago con ese nombre"
                });
            }

            var metodo = new MetodoPago
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo
            };

            _context.MetodosPago.Add(metodo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMetodoPago),
                new { id = metodo.IdMetodoPago },
                metodo);
        }

        // PUT: api/MetodosPago/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMetodoPago(
            int id,
            MetodoPagoDto dto)
        {
            var metodo = await _context.MetodosPago.FindAsync(id);

            if (metodo == null)
            {
                return NotFound(new
                {
                    mensaje = "El método de pago no existe"
                });
            }

            metodo.Nombre = dto.Nombre;
            metodo.Descripcion = dto.Descripcion;
            metodo.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Método de pago actualizado correctamente"
            });
        }

        // DELETE: api/MetodosPago/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMetodoPago(int id)
        {
            var metodo = await _context.MetodosPago.FindAsync(id);

            if (metodo == null)
            {
                return NotFound(new
                {
                    mensaje = "El método de pago no existe"
                });
            }

            var tieneCitas = await _context.Citas
                .AnyAsync(c => c.IdMetodoPago == id);

            if (tieneCitas)
            {
                return BadRequest(new
                {
                    mensaje = "No se puede eliminar porque está asociado a una cita"
                });
            }

            _context.MetodosPago.Remove(metodo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}