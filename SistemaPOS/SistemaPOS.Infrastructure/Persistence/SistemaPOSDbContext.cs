using Microsoft.EntityFrameworkCore;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.Infrastructure.Persistence
{
    public class SistemaPOSDbContext : DbContext
    {
        public SistemaPOSDbContext(DbContextOptions<SistemaPOSDbContext> options) : base(options)
        {
        }

        public DbSet<Sede> Sede { get; set; } = null!;
        public DbSet<Catalogo> Catalogos { get; set; } = null!;
        public DbSet<Caja> Cajas { get; set; } = null!;
        public DbSet<CategoriaProducto> Categorias { get; set; } = null!;
        public DbSet<Venta> Ventas { get; set; } = null!;
        public DbSet<DetalleVenta> DetallesVenta { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Map entity to table names if necessary
        }
    }
}
