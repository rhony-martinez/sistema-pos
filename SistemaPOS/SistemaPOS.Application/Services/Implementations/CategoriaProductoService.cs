using Microsoft.EntityFrameworkCore;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;

namespace SistemaPOS.Application.CategoriasProducto
{
    public class CategoriaProductoService : ICategoriaProductoService
    {
        private readonly SistemaPosContext _context;

        public CategoriaProductoService(SistemaPosContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoriaProducto>> ObtenerCategoriasAsync()
        {
            return await _context.CategoriasProducto
                                 .AsNoTracking()
                                 .ToListAsync();
        }
    }
}
