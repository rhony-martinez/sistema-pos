using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Persistence;
using SistemaPOS.Infrastructure.Repositories;

namespace SistemaPOS.Tests.Repositories
{
    public class SedeRepositoryTests
    {
        private SistemaPOSDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SistemaPOSDbContext>()
                .UseInMemoryDatabase(databaseName: "SistemaPOSTestDB")
                .Options;

            return new SistemaPOSDbContext(options);
        }

        [Fact]
        public async Task BuscarSedeAsync_DeberiaRetornarSedePorId()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Sede.Add(new Sede { SEDE_ID = 1, SEDE_NOMBRE = "Central" });
            await context.SaveChangesAsync();

            var repo = new SedeRepository(context);

            // Act
            var result = await repo.BuscarSedeAsync(1, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Central", result.SEDE_NOMBRE);
        }

        [Fact]
        public async Task BuscarSedeAsync_DeberiaRetornarSedePorNombre()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Sede.Add(new Sede { SEDE_ID = 2, SEDE_NOMBRE = "Norte" });
            await context.SaveChangesAsync();

            var repo = new SedeRepository(context);

            // Act
            var result = await repo.BuscarSedeAsync(null, "norte");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.SEDE_ID);
        }

        [Fact]
        public async Task BuscarSedeAsync_DeberiaRetornarNullSiNoExiste()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repo = new SedeRepository(context);

            // Act
            var result = await repo.BuscarSedeAsync(99, null);

            // Assert
            Assert.Null(result);
        }
    }
}
