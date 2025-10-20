using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleVentaController : ControllerBase
    {
        private readonly SistemaPosContext _context;

        public DetalleVentaController(SistemaPosContext context)
        {
            _context = context;
        }

        // GET: api/DetalleVenta
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var detalles = await _context.DetallesVenta
                .Include(d => d.Producto)
                .Include(d => d.Venta)
                .ToListAsync();
            return Ok(detalles);
        }

        // GET: api/DetalleVenta/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var detalle = await _context.DetallesVenta
                .Include(d => d.Producto)
                .Include(d => d.Venta)
                .FirstOrDefaultAsync(d => d.DetId == id);

            return detalle == null ? NotFound() : Ok(detalle);
        }

        // POST: api/DetalleVenta
        [HttpPost]
        public async Task<IActionResult> Create(DetalleVenta detalle)
        {
            _context.DetallesVenta.Add(detalle);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = detalle.DetId }, detalle);
        }

        // PUT: api/DetalleVenta/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, DetalleVenta detalle)
        {
            if (id != detalle.DetId) return BadRequest();

            _context.Entry(detalle).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/DetalleVenta/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var detalle = await _context.DetallesVenta.FindAsync(id);
            if (detalle == null) return NotFound();

            _context.DetallesVenta.Remove(detalle);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
