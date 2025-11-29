using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using SistemaPOS.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IRevokedTokenRepository _revokedRepo;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepo, IRevokedTokenRepository revokedRepo, IConfiguration config)
    {
        _userRepo = userRepo;
        _revokedRepo = revokedRepo;
        _config = config;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req)
    {
        var user = await _userRepo.GetByUsernameAsync(req.Username);
        if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.UsuClaveHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpireMinutes"] ?? "60"));

        // Build JWT
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UsuUsername),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("uid", user.UsuId.ToString()),
            new Claim(ClaimTypes.Role, user.UsuRol ?? "CAJERO"),
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expires).ToUnixTimeSeconds().ToString())
        };

        // 🔹 Agregar el claim de sede solo si aplica
        if (user.SedeId.HasValue)
        {
            claims.Add(new Claim("sedeId", user.SedeId.Value.ToString()));
        }


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.WriteToken(token);

        return new LoginResponse { AccessToken = jwt, ExpiresAt = expires };
    }

    public async Task LogoutAsync(string tokenJti, DateTime expiresAt)
    {
        // Guardar jti en tabla de revocados
        var revoked = new RevokedToken { Jti = tokenJti, ExpiresAt = expiresAt };
        await _revokedRepo.AddAsync(revoked);
        await _revokedRepo.SaveChangesAsync();
    }
}
