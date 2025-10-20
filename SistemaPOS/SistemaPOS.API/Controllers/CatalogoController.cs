using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly SistemaPosContext _context;

        public CatalogoController(SistemaPosContext context)
        {
            _context = context;
        }

        // GET: api/Catalogo
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var catalogos = await _context.Catalogos
                .Include(c => c.Sede)
                .Include(c => c.Producto)
                .ToListAsync();
            return Ok(catalogos);
        }

        // GET: api/Catalogo/{sedeId}/{proId}
        [HttpGet("{sedeId}/{proId}")]
        public async Task<IActionResult> GetByIds(int sedeId, int proId)
        {
            var catalogo = await _context.Catalogos
                .Include(c => c.Sede)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(c => c.SedeId == sedeId && c.ProId == proId);

            return catalogo == null ? NotFound() : Ok(catalogo);
        }

        // POST: api/Catalogo
        [HttpPost]
        public async Task<IActionResult> Create(Catalogo catalogo)
        {
            _context.Catalogos.Add(catalogo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByIds), new { sedeId = catalogo.SedeId, proId = catalogo.ProId }, catalogo);
        }

        // DELETE: api/Catalogo/{sedeId}/{proId}
        [HttpDelete("{sedeId}/{proId}")]
        public async Task<IActionResult> Delete(int sedeId, int proId)
        {
            var catalogo = await _context.Catalogos
                .FirstOrDefaultAsync(c => c.SedeId == sedeId && c.ProId == proId);
            if (catalogo == null) return NotFound();

            _context.Catalogos.Remove(catalogo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
