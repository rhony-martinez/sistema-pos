using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SistemaPOS.Application.Sedes;
using SistemaPOS.Infrastructure.Db;
using SistemaPOS.Infrastructure.Sedes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SistemaPOS.Application.Sedes;
using SistemaPOS.Infrastructure.Db;
using SistemaPOS.Infrastructure.Sedes;

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