using Microsoft.EntityFrameworkCore;
using SistemasCitasSpa.Models;

namespace SistemasCitasSpa.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<CategoriaServicio> CategoriasServicios { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Terapeuta> Terapeutas { get; set; }
        public DbSet<HorarioTerapeuta> HorariosTerapeutas { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<Cita> Citas { get; set; }
    }
}