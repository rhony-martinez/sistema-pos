using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using SistemaPOS.API.Reports;
using SistemaPOS.Application.DTOs.Venta;
using SistemaPOS.Application.Services.Interfaces;
using SistemaPOS.Domain.Entities;

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

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _ventaService.ObtenerVentasAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var venta = await _ventaService.ObtenerVentaPorIdAsync(id);
            return venta == null ? NotFound() : Ok(venta);
        }

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
                // Usamos 409 Conflict para reglas de negocio (no hay caja abierta / caja no válida)
                return Conflict(new { mensaje = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                // 500 genérico
                return StatusCode(500, new { mensaje = "Error interno del servidor.", detalle = ex.Message });
            }
        }
        [HttpGet("reporte/pdf")]
        public async Task<IActionResult> ReporteVentasPdf([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            // Defaults: últimos 30 días
            var end = (hasta?.Date ?? DateTime.Now.Date).AddDays(1).AddTicks(-1); // fin del día
            var start = desde?.Date ?? end.AddDays(-30).Date;

            var ventasEnumerable = await _ventaService.ObtenerVentasPorRangoAsync(start, end);
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
                    TotalVendido = g.Sum(x => x.DetSubtotal) // tu NotMapped calculado
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


    }
}