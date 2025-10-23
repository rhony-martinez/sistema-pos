using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) { _userService = userService; }

    // Crear usuario: requiere rol ADMIN_GENERAL o ADMIN_LOCAL
    [Authorize(Roles = "ADMIN_GENERAL,ADMIN_LOCAL")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
    {
        try
        {
            req.Rol = req.Rol?.ToUpperInvariant();
            var user = await _userService.CreateUserAsync(req);
            return CreatedAtAction(nameof(GetById), new { id = user.UsuId }, user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // Obtener perfil del usuario logueado
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var uidClaim = User.FindFirst("uid")?.Value;
        if (!int.TryParse(uidClaim, out var uid))
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(uid);
        if (user == null)
            return NotFound(new { message = "Usuario no encontrado" });

        return Ok(new
        {
            user.UsuId,
            user.UsuPrimerNombre,
            user.UsuPrimerApellido,
            user.UsuCorreo,
            user.UsuRol,
            user.SedeId
        });
    }

    [Authorize(Roles = "ADMIN_GENERAL")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [Authorize(Roles = "ADMIN_LOCAL")]
    [HttpGet("cajeros/activos/{sedeId}")]
    public async Task<IActionResult> GetCajerosActivosPorSede(int sedeId)
    {
        var cajeros = await _userService.GetCajerosActivosPorSedeAsync(sedeId);
        return Ok(new { cantidad = cajeros.Count });
    }

    [Authorize(Roles = "ADMIN_GENERAL")]
    [HttpGet("activos/count")]
    public async Task<IActionResult> GetUsuariosActivosCount()
    {
        try
        {
            var cantidad = await _userService.GetUsuariosActivosCountAsync();
            return Ok(new { usuariosActivos = cantidad });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

}
