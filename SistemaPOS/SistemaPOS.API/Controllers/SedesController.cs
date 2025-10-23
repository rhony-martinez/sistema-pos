using Microsoft.AspNetCore.Mvc;
using SistemaPOS.Application.Sedes;
using System.Threading.Tasks;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SedesController : ControllerBase
    {
        private readonly ListarSedesQuery _listar;
        private readonly CrearSedeCommand _crear;

        public SedesController(ListarSedesQuery listar, CrearSedeCommand crear)
        {
            _listar = listar;
            _crear = crear;
        }

        // GET: /api/sedes
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _listar.ExecuteAsync();
            if (!res.Success)
                return StatusCode(500, new { message = "Error al listar sedes" });

            return Ok(res.Value);
        }

        // POST: /api/sedes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SedeCreateDto dto)
        {
            // Validación mínima en API (podemos enriquecer con FluentValidation)
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
                // 201 Created puede incluir location si quieres
                return CreatedAtAction(nameof(GetAll), new { id = result.Value }, new { id = result.Value });
            }

            var err = result.Error ?? string.Empty;
            if (err.Equals(CrearSedeError.Duplicada.ToString(), System.StringComparison.OrdinalIgnoreCase))
                return Conflict(new { message = "Sede duplicada." });

            if (err.Equals(CrearSedeError.DatosInvalidos.ToString(), System.StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = "Datos inválidos." });

            return StatusCode(500, new { message = "No se pudo crear la sede." });
        }

        // DTO para la API (más simple que el ViewModel)
        public class SedeCreateDto
        {
            public string? Nombre { get; set; }
            public string? Direccion { get; set; }
            public string? Ciudad { get; set; }
            public string? Departamento { get; set; }
            public string? Ubicacion { get; set; }
            public string? Telefono { get; set; }
            public string? Correo { get; set; }
            public string? Estado { get; set; }
        }
    }
}
