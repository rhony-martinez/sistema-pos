using SistemaPOS.Domain.Entities;
using System.Linq.Expressions;

public interface IUserRepository
{
    Task<Usuario?> GetByUsernameAsync(string username);
    Task<Usuario?> GetByIdAsync(int id);
    Task AddAsync(Usuario user);
    Task SaveChangesAsync();
    Task<List<Usuario>> FindAsync(Expression<Func<Usuario, bool>> predicate);
}
