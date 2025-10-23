using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaPOS.Domain.Entities;

namespace SistemaPOS.Application.Sedes
{
    public interface ISedeRepository
    {
        Task<IEnumerable<Sede>> ListarAsync();
        Task<bool> ExisteDuplicadaAsync(string nombre, string ciudad);
        Task<long> CrearAsync(Sede sede);
    }
}
