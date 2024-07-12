using ferreteria_catalog.Models;
using Microsoft.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }
    }
}
