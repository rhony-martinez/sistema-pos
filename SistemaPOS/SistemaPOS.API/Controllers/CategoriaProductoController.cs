using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaProductoController : ControllerBase
    {
        private readonly SistemaPosContext _context;

        public CategoriaProductoController(SistemaPosContext context)
        {
            _context = context;
        }

        // GET: api/CategoriaProducto
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _context.CategoriasProducto.ToListAsync());

        // GET: api/CategoriaProducto/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var categoria = await _context.CategoriasProducto.FindAsync(id);
            return categoria == null ? NotFound() : Ok(categoria);
        }

        // POST: api/CategoriaProducto
        [HttpPost]
        public async Task<IActionResult> Create(CategoriaProducto categoria)
        {
            _context.CategoriasProducto.Add(categoria);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = categoria.CatId }, categoria);
        }

        // PUT: api/CategoriaProducto/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoriaProducto categoria)
        {
            if (id != categoria.CatId) return BadRequest();

            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/CategoriaProducto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.CategoriasProducto.FindAsync(id);
            if (categoria == null) return NotFound();

            _context.CategoriasProducto.Remove(categoria);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
