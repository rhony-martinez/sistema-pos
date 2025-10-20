using SistemaPOS.Domain.Entities;

public interface IUserRepository
{
    Task<Usuario?> GetByUsernameAsync(string username);
    Task<Usuario?> GetByIdAsync(int id);
    Task AddAsync(Usuario user);
    Task SaveChangesAsync();
}
