using SistemaPOS.Application.DTOs.Venta;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.Application.Services.Interfaces
{
    public interface IVentaService
    {
        Task<Venta> CrearVentaAsync(VentaCreateDto dto);

        Task<IEnumerable<Venta>> ObtenerVentasAsync();
        Task<IEnumerable<Venta>> ObtenerVentasPorSedeAsync(int sedeId);

        Task<Venta?> ObtenerVentaPorIdAsync(int id);

        Task<IEnumerable<Venta>> ObtenerVentasPorRangoAsync(DateTime desde, DateTime hasta);
        Task<IEnumerable<Venta>> ObtenerVentasPorRangoYSedeAsync(DateTime desde, DateTime hasta, int sedeId);

        Task<bool> EliminarVentaAsync(int venId);
    }
}
