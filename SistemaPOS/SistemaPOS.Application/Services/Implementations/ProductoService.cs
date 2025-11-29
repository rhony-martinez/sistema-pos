using Microsoft.EntityFrameworkCore;
using SistemaPOS.Application.DTOs.Producto;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Application.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

public class ProductoService : IProductoService
{
    private readonly SistemaPosContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProductoService(SistemaPosContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Producto?> CrearProductoAsync(ProductoRequest request)
    {
        // Obtener el usuario y sede del token JWT
        var user = _httpContextAccessor.HttpContext?.User;
        var sedeIdClaim = user?.Claims.FirstOrDefault(c =>
            c.Type.Equals("sedeId", StringComparison.OrdinalIgnoreCase) ||
            c.Type.EndsWith("/sedeid", StringComparison.OrdinalIgnoreCase))?.Value;

        if (string.IsNullOrEmpty(sedeIdClaim))
            throw new UnauthorizedAccessException("No se pudo obtener la sede del usuario autenticado (claim ausente).");

        int sedeId = int.Parse(sedeIdClaim);

        // Buscar categoría
        var categoria = await _context.CategoriasProducto
            .FirstOrDefaultAsync(c => c.CatNombre == request.CatNombre);

        if (categoria == null)
            throw new ArgumentException($"La categoría '{request.CatNombre}' no existe.");

        // Verificar duplicado por nombre y sede
        bool existe = await _context.Productos
            .Where(p => p.ProEstado == "ACTIVO" && p.ProNombre == request.ProNombre)
            .Join(_context.Catalogos,
                p => p.ProId,
                c => c.ProId,
                (p, c) => new { Producto = p, Catalogo = c })
            .AnyAsync(pc => pc.Catalogo.SedeId == sedeId);

        if (existe)
            throw new InvalidOperationException(
                $"Ya existe un producto activo con ese nombre en la sede {sedeId}."
            );

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

        // Iniciar transacción
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1️⃣ Insertar producto
            _context.Productos.Add(nuevo);

            // 2️⃣ Asociar al catálogo
            var catalogo = new Catalogo
            {
                SedeId = sedeId,
                ProId = nuevo.ProId
            };
            _context.Catalogos.Add(catalogo);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

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
