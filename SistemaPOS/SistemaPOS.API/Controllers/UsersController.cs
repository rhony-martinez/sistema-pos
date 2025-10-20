using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) { _userService = userService; }

    // Crear usuario: requiere rol ADMIN_GENERAL o ADMIN_LOCAL
    //[Authorize(Roles = "ADMIN_GENERAL,ADMIN_LOCAL")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
    {
        try
        {
            var user = await _userService.CreateUserAsync(req);
            return CreatedAtAction(nameof(GetById), new { id = user.UsuId }, user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // opcional: ver mi perfil
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var uidClaim = User.FindFirst("uid")?.Value;
        if (!int.TryParse(uidClaim, out var uid)) return Unauthorized();
        // necesitarías un repo para obtener por id (o UserService)
        return Ok(new { userId = uid });
    }

    [Authorize(Roles = "ADMIN_GENERAL")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        // implementa lectura en IUserRepository o UserService
        return Ok(); // placeholder
    }
}
