using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaPOS.Infrastructure.Db;
using SistemaPOS.Infrastructure.Repositories;

namespace SistemaPOS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
        {
            var cs = cfg.GetConnectionString("OracleDb")
                     ?? throw new InvalidOperationException("ConnectionStrings:OracleDb no configurada");

            services.AddSingleton<IOracleConnectionFactory>(new OracleConnectionFactory(cs));
            services.AddScoped<ISedeRepository, OracleSedeRepository>();

            return services;
        }
    }
}