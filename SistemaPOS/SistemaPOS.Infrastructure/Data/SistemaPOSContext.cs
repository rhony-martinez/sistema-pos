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
                entity.ToTable("SEDE");
                entity.HasKey(e => e.SedeId);

                entity.Property(e => e.SedeId)
                      .HasColumnName("SEDE_ID")
                      .ValueGeneratedOnAdd(); // Identity en SQL Server

                entity.Property(e => e.SedeNombre).HasColumnName("SEDE_NOMBRE").HasMaxLength(100).IsRequired();
                entity.Property(e => e.SedeDireccion).HasColumnName("SEDE_DIRECCION").HasMaxLength(150);
                entity.Property(e => e.SedeCiudad).HasColumnName("SEDE_CIUDAD").HasMaxLength(80);
                entity.Property(e => e.SedeDepartamento).HasColumnName("SEDE_DEPARTAMENTO").HasMaxLength(80);
                entity.Property(e => e.SedeUbicacion).HasColumnName("SEDE_UBICACION").HasMaxLength(100);
                entity.Property(e => e.SedeTelefono).HasColumnName("SEDE_TELEFONO").HasMaxLength(20);
                entity.Property(e => e.SedeCorreo).HasColumnName("SEDE_CORREO").HasMaxLength(100);
                entity.Property(e => e.SedeEstado).HasColumnName("SEDE_ESTADO").HasMaxLength(20).HasDefaultValue("ACTIVA");
            });

            //------------------------------------------------------------
            // CAJA
            //------------------------------------------------------------
            modelBuilder.Entity<Caja>(entity =>
            {
                entity.ToTable("CAJA");
                entity.HasKey(e => e.CajaId);

                entity.Property(e => e.CajaId)
                      .HasColumnName("CAJA_ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.CajaEstado).HasColumnName("CAJA_ESTADO").HasMaxLength(20).HasDefaultValue("ABIERTA");
                entity.Property(e => e.SedeId).HasColumnName("SEDE_ID");

                entity.HasOne(e => e.Sede)
                      .WithMany(s => s.Cajas)
                      .HasForeignKey(e => e.SedeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //------------------------------------------------------------
            // CATEGORIA_PRODUCTO
            //------------------------------------------------------------
            modelBuilder.Entity<CategoriaProducto>(entity =>
            {
                entity.ToTable("CATEGORIA_PRODUCTO");
                entity.HasKey(e => e.CatId);

                entity.Property(e => e.CatId)
                      .HasColumnName("CAT_ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.CatNombre)
                      .HasColumnName("CAT_NOMBRE")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(e => e.CatNombre).IsUnique();
            });

            //------------------------------------------------------------
            // PRODUCTO
            //------------------------------------------------------------
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("PRODUCTO");
                entity.HasKey(e => e.ProId);

                entity.Property(e => e.ProId)
                      .HasColumnName("PRO_ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.ProNombre).HasColumnName("PRO_NOMBRE").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ProDescripcion).HasColumnName("PRO_DESCRIPCION").HasMaxLength(200);
                entity.Property(e => e.ProPrecioVenta).HasColumnName("PRO_PRECIO_VENTA").HasColumnType("decimal(12,2)");
                entity.Property(e => e.ProUnidad).HasColumnName("PRO_UNIDAD").HasMaxLength(20);
                entity.Property(e => e.ProEstado).HasColumnName("PRO_ESTADO").HasMaxLength(20).HasDefaultValue("ACTIVO");

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

                entity.Property(e => e.UsuId)
                      .HasColumnName("USU_ID")
                      .ValueGeneratedOnAdd(); // Identity en SQL Server

                entity.Property(e => e.UsuPrimerNombre).HasColumnName("USU_PRIMER_NOMBRE").HasMaxLength(50).IsRequired();
                entity.Property(e => e.UsuSegundoNombre).HasColumnName("USU_SEGUNDO_NOMBRE").HasMaxLength(50);
                entity.Property(e => e.UsuPrimerApellido).HasColumnName("USU_PRIMER_APELLIDO").HasMaxLength(50).IsRequired();
                entity.Property(e => e.UsuSegundoApellido).HasColumnName("USU_SEGUNDO_APELLIDO").HasMaxLength(50);
                entity.Property(e => e.UsuCorreo).HasColumnName("USU_CORREO").HasMaxLength(100).IsRequired();
                entity.Property(e => e.UsuTelefono).HasColumnName("USU_TELEFONO").HasMaxLength(20);
                entity.Property(e => e.UsuUsername).HasColumnName("USU_USERNAME").HasMaxLength(50).IsRequired();
                entity.Property(e => e.UsuClaveHash).HasColumnName("USU_CLAVE_HASH").HasMaxLength(255).IsRequired();
                entity.Property(e => e.UsuEstado).HasColumnName("USU_ESTADO").HasMaxLength(20).HasDefaultValue("ACTIVO");
                entity.Property(e => e.UsuRol).HasColumnName("USU_ROL").HasMaxLength(30);
                entity.Property(e => e.SedeId).HasColumnName("SEDE_ID");

                entity.HasOne(e => e.Sede)
                      .WithMany(s => s.Usuarios)
                      .HasForeignKey(e => e.SedeId);

                // 🔹 Índice único condicional (solo un ADMIN_LOCAL por sede)
                entity.HasIndex(e => e.SedeId)
                      .HasFilter("USU_ROL = 'ADMIN_LOCAL'")
                      .IsUnique()
                      .HasDatabaseName("UQ_AdminLocal_Sede");
            });

            //------------------------------------------------------------
            // VENTA
            //------------------------------------------------------------
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("VENTA");
                entity.HasKey(e => e.VenId);

                entity.Property(e => e.VenId)
                      .HasColumnName("VEN_ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.VenTotal).HasColumnName("VEN_TOTAL").HasColumnType("decimal(12,2)").IsRequired();
                entity.Property(e => e.VenMetodoPago).HasColumnName("VEN_METODO_PAGO").HasMaxLength(30).IsRequired();
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

                entity.Property(e => e.DetId)
                      .HasColumnName("DET_ID")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.DetCantidad).HasColumnName("DET_CANTIDAD").HasColumnType("decimal(8,2)");
                entity.Property(e => e.DetPrecioUnitario).HasColumnName("DET_PRECIO_UNITARIO").HasColumnType("decimal(12,2)");
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

            base.OnModelCreating(modelBuilder);
        }
    }
}
