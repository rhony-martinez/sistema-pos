using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Domain.Entities;
=======
using SistemaPOS.Domain.Repositories;
>>>>>>> origin/feature_cedj.dacg_consultar_sede

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SedeController : ControllerBase
    {
<<<<<<< HEAD
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

        [HttpGet("activas/count")]
        public async Task<IActionResult> GetCantidadSedesActivas()
        {
            try
            {
                var cantidad = await _context.Sedes
                    .CountAsync(s => s.SedeEstado.ToUpper() == "ACTIVA");

                return Ok(new { sedesActivas = cantidad });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

=======
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
>>>>>>> origin/feature_cedj.dacg_consultar_sede
    }
}
