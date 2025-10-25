using SistemaPOS.Application.DTOs;
using SistemaPOS.Domain.Entities;

public interface IUserService
{
    Task<Usuario> CreateUserAsync(CreateUserRequest dto);
    Task<Usuario?> GetUserByIdAsync(int id); // agregado
    Task<List<Usuario>> GetCajerosActivosPorSedeAsync(int sedeId);
    Task<List<Usuario>> GetCajerosPorSedeAsync(int sedeId);
    Task<int> GetUsuariosActivosCountAsync();
    Task<List<Usuario>> GetAllUsersAsync();
    Task<bool> UpdateUserAsync(int id, UpdateUserRequest dto);
    Task<Usuario?> GetUserByIdAsyncToUpdate(int id);
    Task<List<Usuario>> GetUsersByRoleAsync(string rol);
}

