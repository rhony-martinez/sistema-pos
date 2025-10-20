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
    }
}
