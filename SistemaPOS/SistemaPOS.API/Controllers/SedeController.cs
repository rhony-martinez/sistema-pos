using Microsoft.AspNetCore.Mvc;
using SistemaPOS.Application.Queries.Sedes;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SedeController : ControllerBase
    {
        private readonly ISedeQueries _queries;

        public SedeController(ISedeQueries queries) => _queries = queries;


        // GET: api/Sede
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _queries.GetAllAsync());
    }
}

