using Microsoft.EntityFrameworkCore;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Infrastructure.Finders.Sedes;


namespace SistemaPOS.Infrastructure.Test.Sedes
{
    public class SedeFinderTest
    {
        private static SistemaPosContext NewCtx()
        {
            var options = new DbContextOptionsBuilder<SistemaPosContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SistemaPosContext(options);
        }

        [Fact]
        public async Task GetAllAsync_sin_sedes_devuelve_vacio()
        {
            using var ctx = NewCtx();
            var sut = new SedeFinder(ctx);

            var list = await sut.GetAllAsync();

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetAllAsync_sede_sin_usuarios_devuelve_lista_usuarios_vacia()
        {
            using var ctx = NewCtx();
            ctx.Sedes.Add(new Sede
            {
                SedeNombre = "SoloSede",
                SedeDireccion = "Av. Colombia #123",
                SedeCiudad = "Bogotá",
                SedeDepartamento = "Cundinamarca",
                SedeUbicacion = "Centro Comercial La luna, Local 14",
                SedeTelefono = "3201234567",
                SedeCorreo = "sur@pos.com"
            });

            await ctx.SaveChangesAsync();

            var sut = new SedeFinder(ctx);
            var dto = Assert.Single(await sut.GetAllAsync());

            Assert.Equal("SoloSede", dto.SedeNombre);
            Assert.Empty(dto.Usuarios);
        }

        [Fact]
        public async Task GetAllAsync_mapea_nulos_a_cadena_vacia()
        {
            using var ctx = NewCtx();
            ctx.Sedes.Add(new Sede
            {
                SedeNombre = "Centro",
                SedeDireccion = null,
                SedeTelefono = null,
                SedeEstado = null
            });
            await ctx.SaveChangesAsync();

            var sut = new SedeFinder(ctx);
            var dto = Assert.Single(await sut.GetAllAsync());

            Assert.Equal(string.Empty, dto.SedeDireccion);
            Assert.Equal(string.Empty, dto.SedeTelefono);
            Assert.Equal(string.Empty, dto.SedeEstado);
        }

        [Fact]
        public async Task GetAllAsync_preserva_campos_no_nulos_y_usuarios_multiples()
        {
            using var ctx = NewCtx();

            var sede = new Sede
            {
                SedeNombre = "Norte",
                SedeDireccion = "Calle 1",
                SedeTelefono = "3001234567",
                SedeEstado = "ACTIVA"
            };
            ctx.Sedes.Add(sede);
            await ctx.SaveChangesAsync();

            ctx.Usuarios.AddRange(
                new Usuario
                {
                    SedeId = sede.SedeId,
                    UsuPrimerNombre = "Ana",
                    UsuPrimerApellido = "Rios",
                    UsuUsername = "ana.rios",
                    UsuRol = "ADMIN_LOCAL",
                    UsuEstado = "ACTIVO",
                    UsuCorreo = "ana@pos.local",
                    UsuClaveHash = "x"
                },
                new Usuario
                {
                    SedeId = sede.SedeId,
                    UsuPrimerNombre = "Luis",
                    UsuPrimerApellido = "Perez",
                    UsuUsername = "luis.perez",
                    UsuRol = "CAJERO",
                    UsuEstado = "ACTIVO",
                    UsuCorreo = "luis@pos.local",
                    UsuClaveHash = "y"
                }
            );
            await ctx.SaveChangesAsync();

            var sut = new SedeFinder(ctx);
            var dto = Assert.Single(await sut.GetAllAsync());

            Assert.Equal("Calle 1", dto.SedeDireccion);
            Assert.Equal("3001234567", dto.SedeTelefono);
            Assert.Equal("ACTIVA", dto.SedeEstado);

            Assert.Equal(2, dto.Usuarios.Count);
            Assert.Contains(dto.Usuarios, u => u.NombreCompleto == "Ana Rios" && u.Username == "ana.rios");
            Assert.Contains(dto.Usuarios, u => u.NombreCompleto == "Luis Perez" && u.Username == "luis.perez");
        }

        [Fact]
        public async Task GetAllAsync_ordenado_por_SedeId_asc()
        {
            using var ctx = NewCtx();
            ctx.Sedes.AddRange(new Sede { SedeNombre = "Zeta" }, new Sede { SedeNombre = "Alpha" });
            await ctx.SaveChangesAsync();

            var sut = new SedeFinder(ctx);
            var list = await sut.GetAllAsync();

            Assert.Equal(2, list.Count);
            Assert.True(list[0].SedeId < list[1].SedeId);
        }
    }
}
