using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SedeController : ControllerBase
    {
        private readonly SistemaPosContext _context;

        //Constructor
        public SedeController(SistemaPosContext context, ISedeRepository sedeRepository)
        {
            _context = context;
            _sedeRepository = sedeRepository;
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

        private readonly ISedeRepository _sedeRepository;

      

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
                Console.WriteLine($"✅ Sede encontrada: {sede.SedeNombre} (ID: {sede.SedeId})");

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
        [HttpPost("{id}/eliminar")]
        public async Task<IActionResult> InactivarSede(int id)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();

                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "BEGIN :resultado := fn_inactivar_sede(:p_sede_id); END;";
                    command.CommandType = System.Data.CommandType.Text;

                    var resultadoParam = command.CreateParameter();
                    resultadoParam.ParameterName = "resultado";
                    resultadoParam.DbType = System.Data.DbType.String;
                    resultadoParam.Size = 200;
                    resultadoParam.Direction = System.Data.ParameterDirection.Output;
                    command.Parameters.Add(resultadoParam);

                    var sedeIdParam = command.CreateParameter();
                    sedeIdParam.ParameterName = "p_sede_id";
                    sedeIdParam.Value = id;
                    sedeIdParam.DbType = System.Data.DbType.Int32;
                    command.Parameters.Add(sedeIdParam);

                    await command.ExecuteNonQueryAsync();

                    string mensaje = resultadoParam.Value?.ToString() ?? "Sin respuesta";
                    return Ok(new { message = mensaje });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
