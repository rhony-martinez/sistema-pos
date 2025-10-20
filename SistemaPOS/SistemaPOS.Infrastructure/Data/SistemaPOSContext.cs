using Microsoft.EntityFrameworkCore;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.Infrastructure.Data
{
    public class SistemaPosContext : DbContext
    {
        public SistemaPosContext(DbContextOptions<SistemaPosContext> options)
            : base(options) { }

        // DbSets
        public DbSet<Sede> Sedes { get; set; }
        public DbSet<Caja> Cajas { get; set; }
        public DbSet<CategoriaProducto> CategoriasProducto { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Catalogo> Catalogos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }
        public DbSet<RevokedToken> RevokedTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //------------------------------------------------------------
            // SEDE
            //------------------------------------------------------------
            modelBuilder.Entity<Sede>(entity =>
            {
                entity.HasKey(e => e.SedeId);
                entity.Property(e => e.SedeId).HasColumnName("SEDE_ID");
                entity.Property(e => e.SedeNombre).HasColumnName("SEDE_NOMBRE");
                entity.Property(e => e.SedeEstado).HasColumnName("SEDE_ESTADO");
            });

            //------------------------------------------------------------
            // CAJA
            //------------------------------------------------------------
            modelBuilder.Entity<Caja>(entity =>
            {
                entity.HasKey(e => e.CajaId);
                entity.Property(e => e.CajaId).HasColumnName("CAJA_ID");
                entity.Property(e => e.SedeId).HasColumnName("SEDE_ID");

                entity.HasOne(e => e.Sede)
                      .WithMany(s => s.Cajas)
                      .HasForeignKey(e => e.SedeId);
            });

            //------------------------------------------------------------
            // CATEGORIA_PRODUCTO
            //------------------------------------------------------------
            modelBuilder.Entity<CategoriaProducto>(entity =>
            {
                entity.ToTable("CATEGORIA_PRODUCTO");
                entity.HasKey(e => e.CatId);
                entity.Property(e => e.CatId).HasColumnName("CAT_ID");
                entity.Property(e => e.CatNombre).HasColumnName("CAT_NOMBRE");
            });

            //------------------------------------------------------------
            // PRODUCTO
            //------------------------------------------------------------
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("PRODUCTO");
                entity.HasKey(e => e.ProId);
                entity.Property(e => e.ProId).HasColumnName("PRO_ID");
                entity.Property(e => e.CatId).HasColumnName("CAT_ID");

                entity.HasOne(e => e.Categoria)
                      .WithMany()
                      .HasForeignKey(e => e.CatId);
            });

            //------------------------------------------------------------
            // CATALOGO (clave compuesta)
            //------------------------------------------------------------
            modelBuilder.Entity<Catalogo>(entity =>
            {
                entity.ToTable("CATALOGO");
                entity.HasKey(e => new { e.SedeId, e.ProId });

                entity.Property(e => e.SedeId).HasColumnName("SEDE_ID");
                entity.Property(e => e.ProId).HasColumnName("PRO_ID");

                entity.HasOne(e => e.Sede)
                      .WithMany(s => s.Catalogos)
                      .HasForeignKey(e => e.SedeId);

                entity.HasOne(e => e.Producto)
                      .WithMany()
                      .HasForeignKey(e => e.ProId);
            });

            //------------------------------------------------------------
            // USUARIO
            //------------------------------------------------------------
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIO");
                entity.HasKey(e => e.UsuId);
                entity.Property(e => e.UsuId).HasColumnName("USU_ID");
                entity.Property(e => e.SedeId).HasColumnName("SEDE_ID");

                entity.HasOne(e => e.Sede)
                      .WithMany(s => s.Usuarios)
                      .HasForeignKey(e => e.SedeId);
            });

            //------------------------------------------------------------
            // VENTA
            //------------------------------------------------------------
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("VENTA");
                entity.HasKey(e => e.VenId);
                entity.Property(e => e.VenId).HasColumnName("VEN_ID");
                entity.Property(e => e.CajaId).HasColumnName("CAJA_ID");

                entity.HasOne(e => e.Caja)
                      .WithMany()
                      .HasForeignKey(e => e.CajaId);
            });

            //------------------------------------------------------------
            // DETALLE_VENTA
            //------------------------------------------------------------
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("DETALLE_VENTA");
                entity.HasKey(e => e.DetId);
                entity.Property(e => e.DetId).HasColumnName("DET_ID");
                entity.Property(e => e.VenId).HasColumnName("VEN_ID");
                entity.Property(e => e.ProId).HasColumnName("PRO_ID");

                entity.HasOne(e => e.Venta)
                      .WithMany(v => v.Detalles)
                      .HasForeignKey(e => e.VenId);

                entity.HasOne(e => e.Producto)
                      .WithMany()
                      .HasForeignKey(e => e.ProId);
            });

            //------------------------------------------------------------
            // REVOKED_TOKEN
            //------------------------------------------------------------
            modelBuilder.Entity<RevokedToken>(entity =>
            {
                entity.ToTable("REVOKED_TOKEN");
                entity.HasKey(r => r.Jti);
                entity.Property(r => r.Jti).HasColumnName("JT_ID");
                entity.Property(r => r.ExpiresAt).HasColumnName("EXPIRES_AT");
            });

            //------------------------------------------------------------
            // BASE
            //------------------------------------------------------------
            base.OnModelCreating(modelBuilder);

        }
    }
}
