using Microsoft.EntityFrameworkCore;
using SistemaPOS.Application.DTOs.Producto;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Application.Services.Interfaces;


namespace SistemaPOS.Application.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private readonly SistemaPosContext _context;

        public ProductoService(SistemaPosContext context)
        {
            _context = context;
        }

        public async Task<Producto?> CrearProductoAsync(ProductoRequest request)
        {
            // Buscar categoría por nombre
            var categoria = await _context.CategoriasProducto
                .FirstOrDefaultAsync(c => c.CatNombre == request.CatNombre);

            if (categoria == null)
                throw new ArgumentException($"La categoría '{request.CatNombre}' no existe.");

            // Verificar duplicado
            bool existe = await _context.Productos
                .AnyAsync(p => p.ProNombre == request.ProNombre && p.ProEstado == "ACTIVO");
            if (existe)
                throw new InvalidOperationException("Ya existe un producto activo con ese nombre.");

            // Crear producto
            var nuevo = new Producto
            {
                ProNombre = request.ProNombre,
                ProDescripcion = request.ProDescripcion,
                ProPrecioVenta = Math.Round(request.ProPrecioVenta, 2),
                ProUnidad = request.ProUnidad,
                ProEstado = "ACTIVO",
                CatId = categoria.CatId
            };

            _context.Productos.Add(nuevo);
            await _context.SaveChangesAsync();

            return nuevo;
        }
        public async Task<Producto?> GetByIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria) // opcional, si quieres traer datos de categoría
                .FirstOrDefaultAsync(p => p.ProId == id && p.ProEstado == "ACTIVO");
        }


        public async Task<IEnumerable<ProductoResponse>> ObtenerProductosAsync()
        {
            var productos = await _context.Productos
                .Include(p => p.Categoria) // 👈 importante: asume que tienes relación navigation property
                .Where(p => p.ProEstado == "ACTIVO")
                .ToListAsync();

            return productos.Select(p => new ProductoResponse
            {
                ProId = p.ProId,
                ProNombre = p.ProNombre,
                ProDescripcion = p.ProDescripcion,
                ProPrecioVenta = p.ProPrecioVenta,
                ProUnidad = p.ProUnidad,
                CatNombre = p.Categoria != null ? p.Categoria.CatNombre : "(Sin categoría)"
            });
        }


    }
}