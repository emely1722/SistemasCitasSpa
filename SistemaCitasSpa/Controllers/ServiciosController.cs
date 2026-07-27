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
    public class ServiciosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiciosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Servicios
        [HttpGet]
        public async Task<IActionResult> GetServicios()
        {
            var servicios = await _context.Servicios
                .AsNoTracking()
                .Select(s => new
                {
                    s.IdServicio,
                    s.Nombre,
                    s.Descripcion,
                    s.Precio,
                    s.DuracionMinutos,
                    s.IdCategoria,
                    Categoria = s.Categoria != null
                        ? s.Categoria.Nombre
                        : null,
                    s.Activo
                })
                .ToListAsync();

            return Ok(servicios);
        }

        // GET: api/Servicios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServicio(int id)
        {
            var servicio = await _context.Servicios
                .AsNoTracking()
                .Where(s => s.IdServicio == id)
                .Select(s => new
                {
                    s.IdServicio,
                    s.Nombre,
                    s.Descripcion,
                    s.Precio,
                    s.DuracionMinutos,
                    s.IdCategoria,
                    Categoria = s.Categoria != null
                        ? s.Categoria.Nombre
                        : null,
                    s.Activo
                })
                .FirstOrDefaultAsync();

            if (servicio == null)
            {
                return NotFound(new
                {
                    mensaje = "El servicio no existe"
                });
            }

            return Ok(servicio);
        }

        // POST: api/Servicios
        [HttpPost]
        public async Task<IActionResult> PostServicio(ServicioDto dto)
        {
            var categoriaExiste = await _context.CategoriasServicios
                .AnyAsync(c => c.IdCategoria == dto.IdCategoria);

            if (!categoriaExiste)
            {
                return BadRequest(new
                {
                    mensaje = "La categoría no existe"
                });
            }

            var existe = await _context.Servicios
                .AnyAsync(s => s.Nombre == dto.Nombre);

            if (existe)
            {
                return Conflict(new
                {
                    mensaje = "Ya existe un servicio con ese nombre"
                });
            }

            var servicio = new Servicio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                DuracionMinutos = dto.DuracionMinutos,
                IdCategoria = dto.IdCategoria,
                Activo = dto.Activo
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetServicio),
                new { id = servicio.IdServicio },
                new
                {
                    servicio.IdServicio,
                    servicio.Nombre,
                    servicio.Descripcion,
                    servicio.Precio,
                    servicio.DuracionMinutos,
                    servicio.IdCategoria,
                    servicio.Activo
                });
        }

        // PUT: api/Servicios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServicio(
            int id,
            ServicioDto dto)
        {
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null)
            {
                return NotFound(new
                {
                    mensaje = "El servicio no existe"
                });
            }

            var categoriaExiste = await _context.CategoriasServicios
                .AnyAsync(c => c.IdCategoria == dto.IdCategoria);

            if (!categoriaExiste)
            {
                return BadRequest(new
                {
                    mensaje = "La categoría no existe"
                });
            }

            servicio.Nombre = dto.Nombre;
            servicio.Descripcion = dto.Descripcion;
            servicio.Precio = dto.Precio;
            servicio.DuracionMinutos = dto.DuracionMinutos;
            servicio.IdCategoria = dto.IdCategoria;
            servicio.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Servicio actualizado correctamente"
            });
        }

        // DELETE: api/Servicios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicio(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null)
            {
                return NotFound(new
                {
                    mensaje = "El servicio no existe"
                });
            }

            var tieneCitas = await _context.Citas
                .AnyAsync(c => c.IdServicio == id);

            if (tieneCitas)
            {
                return BadRequest(new
                {
                    mensaje = "No se puede eliminar porque el servicio tiene citas registradas"
                });
            }

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}