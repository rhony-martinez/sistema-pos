using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Infrastructure.Repositories;

namespace SistemaPOS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
        {
            // ✅ Conexión a SQL Server
            var connectionString = cfg.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection no configurada");

            // ✅ Registrar el DbContext de EF Core
            services.AddDbContext<SistemaPosContext>(options =>
                options.UseSqlServer(connectionString));

            // ✅ Registrar repositorios reales
            services.AddScoped<ISedeRepository, SedeRepository>();

            return services;
        }
    }
}
