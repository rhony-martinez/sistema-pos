using SistemaPOS.Domain.Entities;

namespace SistemaPOS.Application.CategoriasProducto
{
    public interface ICategoriaProductoService
    {
        Task<IEnumerable<CategoriaProducto>> ObtenerCategoriasAsync();
    }
}
