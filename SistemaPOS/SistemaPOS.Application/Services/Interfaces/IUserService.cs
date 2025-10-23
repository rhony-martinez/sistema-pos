using SistemaPOS.Domain.Entities;

public interface IUserService
{
    Task<Usuario> CreateUserAsync(CreateUserRequest dto);
    Task<Usuario?> GetUserByIdAsync(int id); // agregado
    Task<List<Usuario>> GetCajerosActivosPorSedeAsync(int sedeId);
    Task<int> GetUsuariosActivosCountAsync();
}
