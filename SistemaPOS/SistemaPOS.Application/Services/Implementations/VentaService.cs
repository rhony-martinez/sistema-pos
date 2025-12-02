using SistemaPOS.Application.DTOs.Venta;
using SistemaPOS.Application.Services.Interfaces;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SistemaPOS.Application.Services.Implementations
{
    public class VentaService : IVentaService
    {
        private readonly SistemaPosContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VentaService(SistemaPosContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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

            // 🔍 Obtener sede desde JWT (mediante IHttpContextAccessor)
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                throw new UnauthorizedAccessException("Usuario no autenticado.");

            var sedeIdClaim = user.Claims.FirstOrDefault(c =>
                c.Type.Equals("sedeId", StringComparison.OrdinalIgnoreCase) ||
                c.Type.EndsWith("/sedeid", StringComparison.OrdinalIgnoreCase))?.Value;

            if (string.IsNullOrEmpty(sedeIdClaim))
                throw new UnauthorizedAccessException("No se pudo obtener la sede del usuario autenticado.");

            if (!int.TryParse(sedeIdClaim, out var sedeId))
                throw new UnauthorizedAccessException("Claim de sede inválido.");

            // 🔍 Verificar caja abierta en la sede
            var cajaAbierta = await _context.Cajas
                .FirstOrDefaultAsync(c =>
                    c.SedeId == sedeId &&
                    c.CajaEstado == "ABIERTA");

            if (cajaAbierta == null)
                throw new InvalidOperationException("No hay una caja abierta en esta sede.");

            // 🔍 El cajero debe usar la caja abierta real
            if (dto.CajaId != cajaAbierta.CajaId)
                throw new InvalidOperationException("La caja seleccionada no está abierta.");

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
