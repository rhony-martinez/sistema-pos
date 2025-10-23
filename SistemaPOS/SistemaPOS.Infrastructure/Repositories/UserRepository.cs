using Microsoft.EntityFrameworkCore;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;
using System.Linq.Expressions;

public class UserRepository : IUserRepository
{
    private readonly SistemaPosContext _ctx;

    public UserRepository(SistemaPosContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Usuario?> GetByUsernameAsync(string username) =>
        await _ctx.Usuarios.FirstOrDefaultAsync(u => u.UsuUsername == username);

    public async Task<Usuario?> GetByIdAsync(int id) =>
        await _ctx.Usuarios.FindAsync(id);

    public async Task AddAsync(Usuario user)
    {
      
        await _ctx.Usuarios.AddAsync(user);
    }

    public async Task SaveChangesAsync() =>
        await _ctx.SaveChangesAsync();

    public async Task<List<Usuario>> FindAsync(Expression<Func<Usuario, bool>> predicate)
    {
        return await _ctx.Usuarios
            .Where(predicate)
            .ToListAsync();
    }
}
