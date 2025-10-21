using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) { _auth = auth; }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        try
        {
            var res = await _auth.LoginAsync(req);
            return Ok(res);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
        if (string.IsNullOrEmpty(jti)) return BadRequest("Token JTI not found.");

        var expClaim = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        DateTime expiresAt;

        if (!string.IsNullOrEmpty(expClaim))
        {
            // Exp viene como epoch seconds
            if (!long.TryParse(expClaim, out var expEpoch))
            {
                // fallback: usa expiración actual + 1 hora
                expiresAt = DateTime.UtcNow.AddHours(1);
            }
            else
            {
                expiresAt = DateTimeOffset.FromUnixTimeSeconds(expEpoch).UtcDateTime;
            }
        }
        else
        {
            // fallback si claim no existe
            expiresAt = DateTime.UtcNow.AddHours(1);
        }

        await _auth.LogoutAsync(jti, expiresAt);
        return NoContent();
    }

    [Authorize]
    [HttpGet("validate")]
    public IActionResult Validate() => Ok(new { valid = true });

}