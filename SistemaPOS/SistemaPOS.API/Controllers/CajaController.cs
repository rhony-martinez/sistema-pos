using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Domain.Entities;

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


    }
}
