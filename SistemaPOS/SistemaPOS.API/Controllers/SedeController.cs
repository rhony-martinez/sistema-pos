using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SedeController : ControllerBase
    {
        private readonly SistemaPosContext _context;

        public SedeController(SistemaPosContext context)
        {
            _context = context;
        }

        // GET: api/Sede
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sedes = await _context.Sedes.ToListAsync();
            return Ok(sedes);
        }
    }
}
