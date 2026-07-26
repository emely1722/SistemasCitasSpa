using Microsoft.EntityFrameworkCore;
using SistemasCitasSpa.Models;

namespace SistemasCitasSpa.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();

            modelBuilder.Entity<CategoriaServicio>()
                .HasIndex(c => c.Nombre)
                .IsUnique();

            modelBuilder.Entity<Servicio>()
                .HasIndex(s => s.Nombre)
                .IsUnique();

            modelBuilder.Entity<Sala>()
                .HasIndex(s => s.Nombre)
                .IsUnique();

            modelBuilder.Entity<MetodoPago>()
                .HasIndex(m => m.Nombre)
                .IsUnique();

            modelBuilder.Entity<Servicio>()
                .Property(s => s.Precio)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Servicio>()
                .HasOne(s => s.Categoria)
                .WithMany(c => c.Servicios)
                .HasForeignKey(s => s.IdCategoria)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<HorarioTerapeuta>()
                .HasOne(h => h.Terapeuta)
                .WithMany(t => t.Horarios)
                .HasForeignKey(h => h.IdTerapeuta)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Cliente)
                .WithMany(c => c.Citas)
                .HasForeignKey(c => c.IdCliente)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Servicio)
                .WithMany(s => s.Citas)
                .HasForeignKey(c => c.IdServicio)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Terapeuta)
                .WithMany(t => t.Citas)
                .HasForeignKey(c => c.IdTerapeuta)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Sala)
                .WithMany(s => s.Citas)
                .HasForeignKey(c => c.IdSala)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.MetodoPago)
                .WithMany(m => m.Citas)
                .HasForeignKey(c => c.IdMetodoPago)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}