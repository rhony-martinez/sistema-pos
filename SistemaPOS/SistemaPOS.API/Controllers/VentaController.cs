using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using SistemaPOS.API.Reports;
using SistemaPOS.Application.DTOs.Venta;
using SistemaPOS.Application.Services.Interfaces;
using SistemaPOS.Domain.Entities;
using System.Security.Claims;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        // ✅ Filtra por sede según rol
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // ADMIN_GENERAL -> ve todo
            if (role == "ADMIN_GENERAL")
                return Ok(await _ventaService.ObtenerVentasAsync());

            // ADMIN_LOCAL -> solo su sede
            var sedeIdStr = User.FindFirst("sedeId")?.Value;
            if (string.IsNullOrWhiteSpace(sedeIdStr) || !int.TryParse(sedeIdStr, out var sedeId))
                return Forbid();

            return Ok(await _ventaService.ObtenerVentasPorSedeAsync(sedeId));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var venta = await _ventaService.ObtenerVentaPorIdAsync(id);
            return venta == null ? NotFound() : Ok(venta);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VentaCreateDto dto)
        {
            try
            {
                var venta = await _ventaService.CrearVentaAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = venta.VenId }, venta);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", detalle = ex.Message });
            }
        }

        // ✅ PDF filtrado también por sede según rol
        [Authorize]
        [HttpGet("reporte/pdf")]
        public async Task<IActionResult> ReporteVentasPdf([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            var end = (hasta?.Date ?? DateTime.Now.Date).AddDays(1).AddTicks(-1);
            var start = desde?.Date ?? end.AddDays(-30).Date;

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            IEnumerable<Venta> ventasEnumerable;

            if (role == "ADMIN_GENERAL")
            {
                ventasEnumerable = await _ventaService.ObtenerVentasPorRangoAsync(start, end);
            }
            else
            {
                var sedeIdStr = User.FindFirst("sedeId")?.Value;
                if (string.IsNullOrWhiteSpace(sedeIdStr) || !int.TryParse(sedeIdStr, out var sedeId))
                    return Forbid();

                ventasEnumerable = await _ventaService.ObtenerVentasPorRangoYSedeAsync(start, end, sedeId);
            }

            var ventasList = ventasEnumerable.ToList();

            var ventasNetas = ventasList.Sum(v => v.VenTotal);
            var ventasEfectivo = ventasList.Where(v => v.VenMetodoPago == "Efectivo").Sum(v => v.VenTotal);
            var ventasTarjeta = ventasList.Where(v => v.VenMetodoPago == "Tarjeta").Sum(v => v.VenTotal);
            var ventasTransferencia = ventasList.Where(v => v.VenMetodoPago == "Transferencia").Sum(v => v.VenTotal);

            var productos = ventasList
                .SelectMany(v => v.Detalles ?? new List<DetalleVenta>())
                .GroupBy(d => new { d.ProId, Nombre = d.Producto != null ? d.Producto.ProNombre : "—" })
                .Select(g => new VentasRangoReportData.ProductoRow
                {
                    ProId = g.Key.ProId,
                    Nombre = g.Key.Nombre,
                    Cantidad = g.Sum(x => x.DetCantidad),
                    TotalVendido = g.Sum(x => x.DetSubtotal)
                })
                .ToList();

            var data = new VentasRangoReportData
            {
                Desde = start,
                Hasta = end,

                VentasNetas = ventasNetas,
                CantidadVentas = ventasList.Count,
                TicketPromedio = ventasList.Count > 0 ? ventasNetas / ventasList.Count : 0m,

                VentasEfectivo = ventasEfectivo,
                VentasTarjeta = ventasTarjeta,
                VentasTransferencia = ventasTransferencia,

                Ventas = ventasList.Select(v => new VentasRangoReportData.VentaRow
                {
                    VenId = v.VenId,
                    FechaVenta = v.FechaVenta,
                    MetodoPago = v.VenMetodoPago,
                    Total = v.VenTotal
                }).ToList(),

                Productos = productos
            };

            var pdfBytes = new VentasRangoReportPdf(data).GeneratePdf();
            var fileName = $"ReporteVentas_{start:yyyyMMdd}_a_{end:yyyyMMdd}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var ok = await _ventaService.EliminarVentaAsync(id);
                if (!ok) return NotFound(new { mensaje = $"No se encontró la venta con ID {id}." });

                return Ok(new { mensaje = "Venta eliminada con éxito." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", detalle = ex.Message });
            }
        }
    }
}
