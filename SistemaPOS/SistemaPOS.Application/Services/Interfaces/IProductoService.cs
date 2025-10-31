using SistemaPOS.Application.DTOs.Producto;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.Application.Services.Interfaces
{
    public interface IProductoService
    {
        Task<Producto?> CrearProductoAsync(ProductoRequest request);
        Task<Producto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductoResponse>> ObtenerProductosAsync();

    }
}
