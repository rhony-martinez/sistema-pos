using SistemaPOS.Domain.Entities;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<Usuario> CreateUserAsync(CreateUserRequest dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
            throw new ArgumentException("El nombre de usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("La contraseña es obligatoria.");

        if (dto.UsuId <= 0)
            throw new ArgumentException("El ID del usuario debe ser mayor que 0.");

        var existingUser = await _userRepo.GetByUsernameAsync(dto.Username);
        if (existingUser != null)
            throw new InvalidOperationException($"El nombre de usuario '{dto.Username}' ya existe.");

        var existingId = await _userRepo.GetByIdAsync(dto.UsuId);
        if (existingId != null)
            throw new InvalidOperationException($"El ID '{dto.UsuId}' ya existe.");

        var user = new Usuario
        {
            UsuId = dto.UsuId,
            UsuPrimerNombre = dto.PrimerNombre,
            UsuSegundoNombre = dto.SegundoNombre,
            UsuPrimerApellido = dto.PrimerApellido,
            UsuSegundoApellido = dto.SegundoApellido,
            UsuCorreo = dto.Correo,
            UsuTelefono = dto.Telefono,
            UsuUsername = dto.Username,
            UsuClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            UsuEstado = "ACTIVO",
            UsuRol = dto.Rol?.ToUpperInvariant(),
            SedeId = dto.SedeId
        };

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        return user;
    }

    // Nuevo método para obtener usuario por ID
    public async Task<Usuario?> GetUserByIdAsync(int id)
    {
        return await _userRepo.GetByIdAsync(id);
    }

    public async Task<List<Usuario>> GetCajerosActivosPorSedeAsync(int sedeId)
    {
        return await _userRepo
            .FindAsync(u => u.UsuRol == "CAJERO" && u.SedeId == sedeId && u.UsuEstado == "ACTIVO");
    }

    public async Task<int> GetUsuariosActivosCountAsync()
    {
        return await _userRepo.CountAsync(u =>
            (u.UsuRol == "CAJERO" || u.UsuRol == "ADMIN_LOCAL")
            && u.UsuEstado == "ACTIVO");
    }

}
