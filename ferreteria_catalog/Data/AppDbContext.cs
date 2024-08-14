using ferreteria_catalog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.RegularExpressions;

namespace ferreteria_catalog.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Producto> Producto { get; set; }
        public DbSet<Marca> Marca { get; set; }
        public DbSet<Existencia> Existencia { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Rol> Role { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<RolModulo> RolModulo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Marca)
                .WithMany()
                .HasForeignKey(p => p.MarcaId);
            
            modelBuilder.Entity<Existencia>()
                .HasKey(e => e.ProductoId);

            modelBuilder.Entity<Existencia>()
                .HasOne(e => e.Producto)
                .WithOne(p => p.Existencia)
                .HasForeignKey<Existencia>(e => e.ProductoId);

            // Configuración para la entidad Usuario
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.UsuarioId);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuario)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict); // Configuración del comportamiento de eliminación

            // Configuración para la entidad Modulo
            modelBuilder.Entity<Modulo>()
                .HasKey(m => m.ModuloId);

            modelBuilder.Entity<Modulo>()
                .HasMany(m => m.RolModulo)
                .WithOne(rm => rm.Modulo)
                .HasForeignKey(rm => rm.ModuloId)
                .OnDelete(DeleteBehavior.Cascade); // Configuración del comportamiento de eliminación

            // Configuración para la entidad RolModulo (ya configurada)
            modelBuilder.Entity<RolModulo>()
                .HasKey(rm => new { rm.RolId, rm.ModuloId });

            modelBuilder.Entity<RolModulo>()
                .HasOne(rm => rm.Rol)
                .WithMany(r => r.RolModulo)
                .HasForeignKey(rm => rm.RolId);

            modelBuilder.Entity<RolModulo>()
                .HasOne(rm => rm.Modulo)
                .WithMany(m => m.RolModulo)
                .HasForeignKey(rm => rm.ModuloId);
        }
    }
}
