using SistemaPOS.Application.DTOs.Venta;
using SistemaPOS.Domain.Entities;
using System.Threading.Tasks;

namespace SistemaPOS.Application.Services.Interfaces
{
    public interface IVentaService
    {
        Task<Venta> CrearVentaAsync(VentaCreateDto dto);
        Task<IEnumerable<Venta>> ObtenerVentasAsync();
        Task<Venta?> ObtenerVentaPorIdAsync(int id);
        Task<IEnumerable<Venta>> ObtenerVentasPorRangoAsync(DateTime desde, DateTime hasta);

    }
}
