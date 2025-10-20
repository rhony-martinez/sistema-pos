using SistemaPOS.Domain.Entities;

public interface IRevokedTokenRepository
{
    Task AddAsync(RevokedToken token);
    Task<bool> IsRevokedAsync(string jti);
    Task SaveChangesAsync();
}
