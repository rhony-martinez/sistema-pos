using Microsoft.AspNetCore.Mvc;
using SistemaPOS.Domain.Repositories;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SedeController : ControllerBase
    {
        private readonly ISedeRepository _sedeRepository;

        public SedeController(ISedeRepository sedeRepository)
        {
            _sedeRepository = sedeRepository;
        }

        // ✅ Endpoint para buscar una sede por ID o nombre
        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarSede([FromQuery] int? id, [FromQuery] string? nombre)
        {
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"🔍 DEBUG => id={id}, nombre={nombre}");
            Console.WriteLine("----------------------------------------------------");

            // Validación: debe llegar al menos un parámetro
            if (id == null && string.IsNullOrWhiteSpace(nombre))
                return BadRequest("Debe proporcionar el ID o el nombre de la sede.");

            try
            {
                // Llamamos al repositorio con los parámetros
                var sede = await _sedeRepository.BuscarSedeAsync(id, nombre);

                // Verificamos si se encontró algo
                if (sede == null)
                {
                    Console.WriteLine("⚠️ No se encontró la sede con los datos proporcionados.");
                    return NotFound("No se encontró la sede.");
                }

                // Log de éxito
                Console.WriteLine($"✅ Sede encontrada: {sede.SEDE_NOMBRE} (ID: {sede.SEDE_ID})");

                // Retornamos el objeto encontrado
                return Ok(sede);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR EN BuscarSede:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
        }
    }
}
