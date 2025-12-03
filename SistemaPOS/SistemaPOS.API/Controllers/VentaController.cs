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

    }
}
