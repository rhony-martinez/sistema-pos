using Microsoft.EntityFrameworkCore;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;

public class RevokedTokenRepository : IRevokedTokenRepository
{
    private readonly SistemaPosContext _ctx;
    public RevokedTokenRepository(SistemaPosContext ctx) { _ctx = ctx; }

    public async Task AddAsync(RevokedToken token)
    {
        await _ctx.RevokedTokens.AddAsync(token);
    }

    public async Task<bool> IsRevokedAsync(string jti) =>
        await _ctx.RevokedTokens.AnyAsync(t => t.Jti == jti);

    public async Task SaveChangesAsync() => await _ctx.SaveChangesAsync();
}
