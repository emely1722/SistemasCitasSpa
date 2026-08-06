using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemasCitasSpa.Data;
using SistemasCitasSpa.DTOs;
using SistemasCitasSpa.Models;

namespace SistemasCitasSpa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriasServiciosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasServiciosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CategoriasServicios
        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            var categorias = await _context.CategoriasServicios
                .AsNoTracking()
                .ToListAsync();

            return Ok(categorias);
        }

        // GET: api/CategoriasServicios/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoria(int id)
        {
            var categoria = await _context.CategoriasServicios
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
                return NotFound(new { mensaje = "La categoría no existe" });

            return Ok(categoria);
        }

        // POST: api/CategoriasServicios
        [HttpPost]
        public async Task<IActionResult> PostCategoria(CategoriaDto dto)
        {
            var existe = await _context.CategoriasServicios
                .AnyAsync(c => c.Nombre == dto.Nombre);

            if (existe)
                return BadRequest(new
                {
                    mensaje = "Ya existe una categoría con ese nombre"
                });

            var categoria = new CategoriaServicio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo
            };

            _context.CategoriasServicios.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategoria),
                new { id = categoria.IdCategoria },
                categoria);
        }

        // PUT: api/CategoriasServicios
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(
            int id,
            CategoriaDto dto)
        {
            var categoria = await _context.CategoriasServicios
                .FindAsync(id);

            if (categoria == null)
                return NotFound(new { mensaje = "La categoría no existe" });

            var existe = await _context.CategoriasServicios
                .AnyAsync(c =>
                    c.Nombre == dto.Nombre &&
                    c.IdCategoria != id);

            if (existe)
                return BadRequest(new
                {
                    mensaje = "Ya existe otra categoría con ese nombre"
                });

            categoria.Nombre = dto.Nombre;
            categoria.Descripcion = dto.Descripcion;
            categoria.Activo = dto.Activo;

            await _context.SaveChangesAsync();

            return Ok(categoria);
        }

        // DELETE: api/CategoriasServicios
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.CategoriasServicios
                .FindAsync(id);

            if (categoria == null)
                return NotFound(new { mensaje = "La categoría no existe" });

            var tieneServicios = await _context.Servicios
                .AnyAsync(s => s.IdCategoria == id);

            if (tieneServicios)
                return BadRequest(new
                {
                    mensaje = "No se puede eliminar porque la categoría tiene servicios registrados"
                });

            _context.CategoriasServicios.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}