using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Application.Sedes;
using SistemaPOS.Application.Services;
using SistemaPOS.Infrastructure.Data;
//using static SistemaPOS.API.Controllers.SedesController;
using SistemaPOS.Application.DTOs;
//using SistemaPOS.Application.DTOs;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SedeController : ControllerBase
    {
        private readonly SistemaPosContext _context;
        private readonly ISedeRepository _sedeRepository;
        private readonly ISedeService _sedeService;
        private readonly ListarSedesQuery _listar;
        private readonly CrearSedeCommand _crear;

        //Constructor
        public SedeController(
            SistemaPosContext context,
            ISedeRepository sedeRepository,
            ISedeService sedeService,
            ListarSedesQuery listar,
            CrearSedeCommand crear)
        {
            _context = context;
            _sedeRepository = sedeRepository;
            _sedeService = sedeService;
            _listar = listar;
            _crear = crear;
        }

        // GET: api/Sede
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _listar.ExecuteAsync();

            if (!res.Success)
                return StatusCode(500, new { message = "Error al listar sedes" });

            return Ok(res.Value);
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

        // Buscar sede
        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarSede([FromQuery] int? id, [FromQuery] string? nombre)
        {
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"🔍 DEBUG => id={id}, nombre={nombre}");
            Console.WriteLine("----------------------------------------------------");

            if (id == null && string.IsNullOrWhiteSpace(nombre))
                return BadRequest("Debe proporcionar el ID o el nombre de la sede.");

            try
            {
                var sede = await _sedeRepository.BuscarSedeAsync(id, nombre);

                if (sede == null)
                {
                    Console.WriteLine("⚠️ No se encontró la sede.");
                    return NotFound("No se encontró la sede.");
                }

                Console.WriteLine($"✅ Sede encontrada: {sede.SedeNombre} (ID: {sede.SedeId})");

                return Ok(sede);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR EN BuscarSede:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
        }

        // Crear sede
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SedeCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { message = "El nombre es obligatorio." });

            var result = await _crear.ExecuteAsync(
                dto.Nombre!,
                dto.Direccion,
                dto.Ciudad,
                dto.Departamento,
                dto.Ubicacion,
                dto.Telefono,
                dto.Correo,
                dto.Estado ?? "ACTIVA");

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value });
            }

            var err = result.Error ?? string.Empty;

            if (err.Equals(CrearSedeError.Duplicada.ToString(), StringComparison.OrdinalIgnoreCase))
                return Conflict(new { message = "Sede duplicada." });

            if (err.Equals(CrearSedeError.DatosInvalidos.ToString(), StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Datos inválidos." });

            return StatusCode(500, new { message = "No se pudo crear la sede." });
        }

        // Inactivar sede
        [HttpPost("{id}/inactivar")]
        public async Task<IActionResult> InactivarSede(int id)
        {
            try
            {
                var mensaje = await _sedeService.InactivarSedeAsync(id);
                return Ok(new { mensaje });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}