using Microsoft.AspNetCore.Mvc;
using SistemaPOS.Application.DTOs.Venta;
using SistemaPOS.Application.Services.Interfaces;

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
            var venta = await _ventaService.CrearVentaAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = venta.VenId }, venta);
        }
    }
}
