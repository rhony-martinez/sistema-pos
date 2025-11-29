using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Application.DTOs.Producto;
using SistemaPOS.Application.Services.Implementations;
using SistemaPOS.Application.Services.Interfaces;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly SistemaPosContext _context;

        public ProductoController(IProductoService productoService, SistemaPosContext context)
        {
            _productoService = productoService;
            _context = context;
        }

        [Authorize(Roles = "ADMIN_LOCAL")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Console.WriteLine($"📥 Producto recibido: {System.Text.Json.JsonSerializer.Serialize(request)}");

            try
            {
                var producto = await _productoService.CrearProductoAsync(request);

                // Armar respuesta limpia
                var response = new ProductoResponse
                {
                    ProId = producto.ProId,
                    ProNombre = producto.ProNombre,
                    ProDescripcion = producto.ProDescripcion,
                    ProPrecioVenta = producto.ProPrecioVenta,
                    ProUnidad = producto.ProUnidad,
                    Categoria = request.CatNombre, // porque la recibes por nombre
                                                   // Sede viene del JWT (no se reenvía, pero el backend ya la usó)
                };

                return CreatedAtAction(nameof(GetById), new { id = producto.ProId }, response);
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
            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null)
                return NotFound(new { mensaje = $"No se encontró el producto con ID {id}." });

            return Ok(producto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var productos = await _productoService.ObtenerProductosAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los productos.", detalle = ex.Message });
            }
        }

    }
}
