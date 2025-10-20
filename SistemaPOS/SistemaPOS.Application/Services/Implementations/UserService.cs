using SistemaPOS.Domain.Entities;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    public UserService(IUserRepository userRepo) { _userRepo = userRepo; }

    public async Task<Usuario> CreateUserAsync(CreateUserRequest dto)
    {
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(dto.Username)) throw new ArgumentException("username required");
        if (string.IsNullOrWhiteSpace(dto.Password)) throw new ArgumentException("password required");

        // check existing user
        var exists = await _userRepo.GetByUsernameAsync(dto.Username);
        if (exists != null) throw new InvalidOperationException("username already exists");

        var user = new Usuario
        {
            UsuPrimerNombre = dto.PrimerNombre,
            UsuSegundoNombre = dto.SegundoNombre,
            UsuPrimerApellido = dto.PrimerApellido,
            UsuSegundoApellido = dto.SegundoApellido,
            UsuCorreo = dto.Correo,
            UsuTelefono = dto.Telefono,
            UsuUsername = dto.Username,
            UsuClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            UsuEstado = "ACTIVO",
            UsuRol = dto.Rol,
            SedeId = dto.SedeId
        };

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();
        return user;
    }
}
