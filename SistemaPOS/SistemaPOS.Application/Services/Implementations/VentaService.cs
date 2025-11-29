using SistemaPOS.Application.DTOs.Venta;
using SistemaPOS.Application.Services.Interfaces;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SistemaPOS.Application.Services.Implementations
{
    public class VentaService : IVentaService
    {
        private readonly SistemaPosContext _context;

        public VentaService(SistemaPosContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Venta>> ObtenerVentasAsync()
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .ToListAsync();
        }

        public async Task<Venta?> ObtenerVentaPorIdAsync(int id)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.VenId == id);
        }

        public async Task<Venta> CrearVentaAsync(VentaCreateDto dto)
        {
            if (dto.Detalles == null || !dto.Detalles.Any())
                throw new InvalidOperationException("La venta debe tener al menos un detalle.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var venta = new Venta
                {
                    VenMetodoPago = dto.VenMetodoPago,
                    CajaId = dto.CajaId,
                    FechaVenta = DateTime.Now,
                    Detalles = dto.Detalles.Select(d => new DetalleVenta
                    {
                        ProId = d.ProId,
                        DetCantidad = d.DetCantidad,
                        DetPrecioUnitario = d.DetPrecioUnitario
                    }).ToList()
                };

                venta.VenTotal = venta.Detalles.Sum(d => d.DetSubtotal);

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return venta;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
