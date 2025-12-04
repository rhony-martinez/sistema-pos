using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using SistemaPOS.API.Reports;
using SistemaPOS.Application.DTOs.Caja;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CajaController : ControllerBase
    {
        private readonly SistemaPosContext _context;

        public CajaController(SistemaPosContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _context.Cajas.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var caja = await _context.Cajas.FindAsync(id);
            return caja == null ? NotFound() : Ok(caja);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Caja caja)
        {
            _context.Cajas.Add(caja);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = caja.CajaId }, caja);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Caja caja)
        {
            if (id != caja.CajaId) return BadRequest();

            _context.Entry(caja).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var caja = await _context.Cajas.FindAsync(id);
            if (caja == null) return NotFound();

            _context.Cajas.Remove(caja);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Devuelve true si hay alguna caja abierta en una sede
        [HttpGet("abierta/{sedeId}")]
        public async Task<IActionResult> HayCajaAbierta(int sedeId)
        {
            try
            {
                // 🔹 Normaliza el valor y usa Trim() para evitar espacios de CHAR
                var abierta = await _context.Cajas
                    .Where(c => c.SedeId == sedeId && c.CajaEstado.Trim().ToUpper() == "ABIERTA")
                    .Select(c => c.CajaId)
                    .FirstOrDefaultAsync();

                bool hayAbierta = abierta != 0;

                return Ok(new { abierta = hayAbierta });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al verificar caja abierta: {ex.Message}");
            }
        }

        [HttpGet("abierta/detalle/{sedeId}")]
        public async Task<IActionResult> ObtenerCajaAbierta(int sedeId)
        {
            var caja = await _context.Cajas
                .Where(c => c.SedeId == sedeId && c.CajaEstado.Trim().ToUpper() == "ABIERTA")
                .FirstOrDefaultAsync();

            if (caja == null)
                return Ok(null); // No hay caja abierta

            return Ok(caja);
        }

        [HttpPost("abrir")]
        public async Task<IActionResult> AbrirCaja([FromBody] AbrirCajaDto dto)
        {
            // Validación básica
            if (dto.MontoInicial <= 0)
                return BadRequest("El monto inicial debe ser mayor a cero.");

            // 🔍 1. Verificar si ya existe caja abierta en la sede
            var cajaAbierta = await _context.Cajas
                .Where(c => c.SedeId == dto.SedeId && c.CajaEstado == "ABIERTA")
                .FirstOrDefaultAsync();

            if (cajaAbierta != null)
                return BadRequest("Ya existe una caja abierta en esta sede.");

            // 🟢 2. Crear nueva caja
            var nuevaCaja = new Caja
            {
                CajaFechaApertura = DateTime.Now,
                CajaMontoInicial = dto.MontoInicial,
                CajaMontoFinal = 0,
                CajaEstado = "ABIERTA",
                SedeId = dto.SedeId
            };

            _context.Cajas.Add(nuevaCaja);
            await _context.SaveChangesAsync();

            return Ok(nuevaCaja);
        }

        [HttpGet("abierta/estado/{sedeId}")]
        public async Task<IActionResult> GetEstadoCajaAbierta(int sedeId)
        {
            var caja = await _context.Cajas
                .Where(c => c.SedeId == sedeId && c.CajaEstado.Trim().ToUpper() == "ABIERTA")
                .FirstOrDefaultAsync();

            if (caja == null) return Ok(null);

            var ventasNetas = await _context.Ventas
                .Where(v => v.CajaId == caja.CajaId)
                .SumAsync(v => (decimal?)v.VenTotal) ?? 0m;

            var montoInicial = caja.CajaMontoInicial ?? 0m;

            // Si aún no tienes ingresos/egresos, quedan en 0
            var ingresosAdicionales = 0m;
            var egresos = 0m;

            var saldoFinalEstimado = montoInicial + ventasNetas + ingresosAdicionales - egresos;

            return Ok(new
            {
                cajaId = caja.CajaId,
                sedeId = caja.SedeId,
                fechaApertura = caja.CajaFechaApertura,
                montoInicial,
                ventasNetas,
                ingresosAdicionales,
                egresos,
                saldoFinalEstimado
            });
        }
        [HttpPost("cerrar/{sedeId}")]
        public async Task<IActionResult> CerrarCaja(int sedeId)
        {
            var caja = await _context.Cajas
                .Where(c => c.SedeId == sedeId && c.CajaEstado.Trim().ToUpper() == "ABIERTA")
                .FirstOrDefaultAsync();

            if (caja == null) return BadRequest(new { mensaje = "No hay caja abierta para cerrar." });

            var ventasNetas = await _context.Ventas
                .Where(v => v.CajaId == caja.CajaId)
                .SumAsync(v => (decimal?)v.VenTotal) ?? 0m;

            var montoInicial = caja.CajaMontoInicial ?? 0m;

            caja.CajaFechaCierre = DateTime.Now;
            caja.CajaMontoFinal = montoInicial + ventasNetas; // luego + ingresos - egresos
            caja.CajaEstado = "CERRADA";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                cajaId = caja.CajaId,
                montoInicial,
                ventasNetas,
                montoFinal = caja.CajaMontoFinal
            });
        }

    [HttpPost("cerrar/{sedeId}/reporte/pdf")]
    public async Task<IActionResult> CerrarCajaYReportePdf(int sedeId)
    {
        var caja = await _context.Cajas
            .Where(c => c.SedeId == sedeId && c.CajaEstado.Trim().ToUpper() == "ABIERTA")
            .OrderByDescending(c => c.CajaFechaApertura)
            .FirstOrDefaultAsync();

        if (caja == null)
            return BadRequest(new { mensaje = "No hay caja abierta para cerrar." });

        var ventas = await _context.Ventas
            .Where(v => v.CajaId == caja.CajaId)
            .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
            .OrderBy(v => v.FechaVenta)
            .ToListAsync();

        var ventasNetas = ventas.Sum(v => v.VenTotal);
        var ventasEfectivo = ventas.Where(v => v.VenMetodoPago == "Efectivo").Sum(v => v.VenTotal);
        var ventasTarjeta = ventas.Where(v => v.VenMetodoPago == "Tarjeta").Sum(v => v.VenTotal);
        var ventasTransferencia = ventas.Where(v => v.VenMetodoPago == "Transferencia").Sum(v => v.VenTotal);

        var montoInicial = caja.CajaMontoInicial ?? 0m;
        var montoFinal = montoInicial + ventasNetas;

        // Cierra la caja
        caja.CajaFechaCierre = DateTime.Now;
        caja.CajaMontoFinal = montoFinal;
        caja.CajaEstado = "CERRADA";
        await _context.SaveChangesAsync();

        // Agrupa productos desde DetalleVenta
        var productos = ventas
            .SelectMany(v => v.Detalles ?? new List<DetalleVenta>())
            .GroupBy(d => new { d.ProId, Nombre = d.Producto != null ? d.Producto.ProNombre : "—" })
            .Select(g => new CajaCierreReportData.ProductoRow
            {
                ProId = g.Key.ProId,
                Nombre = g.Key.Nombre,
                Cantidad = g.Sum(x => x.DetCantidad),
                TotalVendido = g.Sum(x => x.DetSubtotal) // usa tu propiedad calculada
            })
            .ToList();

        var data = new CajaCierreReportData
        {
            CajaId = caja.CajaId,
            SedeId = caja.SedeId,
            FechaApertura = caja.CajaFechaApertura,
            FechaCierre = caja.CajaFechaCierre,

            MontoInicial = montoInicial,
            VentasNetas = ventasNetas,
            VentasEfectivo = ventasEfectivo,
            VentasTarjeta = ventasTarjeta,
            VentasTransferencia = ventasTransferencia,

            CantidadVentas = ventas.Count,
            TicketPromedio = ventas.Count > 0 ? ventasNetas / ventas.Count : 0m,
            MontoFinal = montoFinal,

            Ventas = ventas.Select(v => new CajaCierreReportData.VentaRow
            {
                VenId = v.VenId,
                FechaVenta = v.FechaVenta,
                MetodoPago = v.VenMetodoPago,
                Total = v.VenTotal
            }).ToList(),

            Productos = productos
        };

        var pdfBytes = new CajaCierreReportPdf(data).GeneratePdf();
        var fileName = $"CierreCaja_Sede{caja.SedeId}_Caja{caja.CajaId}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }


}
}

