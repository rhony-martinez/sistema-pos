using System.Collections.Generic;
using System.Threading.Tasks;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.Domain.Repositories
{
    public interface ISedeRepository
    {
        Task<IEnumerable<Sede>> GetAllAsync();
        Task<Sede?> GetByIdAsync(int sedE_ID);
        Task<Sede?> BuscarSedeAsync(int? sedE_ID, string? sedE_NOMBRE);
    }
}



