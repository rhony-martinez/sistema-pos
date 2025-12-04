using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Application.DTOs.Caja;

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


    }
}