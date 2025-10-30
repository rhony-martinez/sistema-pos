using Microsoft.AspNetCore.Mvc;
using SistemaPOS.Application.DTOs.Producto;
using SistemaPOS.Application.Services.Implementations;
using SistemaPOS.Application.Services.Interfaces;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var producto = await _productoService.CrearProductoAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = producto.ProId }, producto);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // ya estaba bien implementado, puedes mantenerlo
            return Ok();
        }
    }
}
