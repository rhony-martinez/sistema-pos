using SistemaPOS.Domain.Entities;

public interface IUserService
{
    Task<Usuario> CreateUserAsync(CreateUserRequest dto);
}
