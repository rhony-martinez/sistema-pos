using Microsoft.AspNetCore.Mvc;
using SistemaPOS.Application.CategoriasProducto;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaProductoController : ControllerBase
    {
        private readonly ICategoriaProductoService _service;

        public CategoriaProductoController(ICategoriaProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            try
            {
                var categorias = await _service.ObtenerCategoriasAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Error interno del servidor.",
                    detalle = ex.Message
                });
            }
        }
    }
}
