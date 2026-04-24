using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaPOS.Application.DTOs;
using SistemaPOS.Infrastructure.Data;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly SistemaPosContext _context;
    public UsersController(SistemaPosContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // Get user by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user == null ? NotFound(new { message = "User not found." }) : Ok(user);
    }

    // Crear usuario: requiere rol ADMIN_GENERAL o ADMIN_LOCAL
    //[Authorize(Roles = "ADMIN_GENERAL,ADMIN_LOCAL")]
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

    // 🔹 Desactivar usuario por ID
    [Authorize(Roles = "ADMIN_GENERAL,ADMIN_LOCAL")]
    [HttpPatch("{id}/desactivar")]
    public async Task<IActionResult> DesactivarUsuario(int id)
    {
        var resultado = await _userService.DeactivateUserAsync(id);

        if (!resultado)
            return NotFound(new { message = "Usuario no encontrado" });

        return Ok(new { message = "Usuario desactivado correctamente" });
        // o si prefieres:
        // return NoContent();
    }

    // Update user
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest dto)
    {
        try
        {
            var actualizado = await _userService.UpdateUserAsync(id, dto);

            if (!actualizado)
                return NotFound(new { message = "Usuario no encontrado." });

            return Ok(new { message = "Usuario actualizado correctamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al actualizar el usuario.",
                error = ex.Message
            });
        }
    }

    [HttpGet("admins-locales")]
    public async Task<IActionResult> GetAdminsLocales()
    {
        var adminsLocales = await _userService.GetUsersByRoleAsync("ADMIN_LOCAL");
        return Ok(adminsLocales);
    }

    [HttpGet("cajeros/{sedeId}")]
    public async Task<IActionResult> GetCajerosPorSede(int sedeId)
    {
        var cajeros = await _userService.GetCajerosPorSedeAsync(sedeId);
        return Ok(cajeros);
    }

}
