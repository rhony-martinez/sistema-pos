using Microsoft.EntityFrameworkCore;
using SistemaPOS.Application.DTOs;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Infrastructure.Data;

public class UserService : IUserService
{
    private readonly SistemaPosContext _context;
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo, SistemaPosContext context)
    {
        _userRepo = userRepo;
        _context = context;
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

    public async Task<Usuario?> GetUserByIdAsyncToUpdate(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<List<Usuario>> GetCajerosActivosPorSedeAsync(int sedeId)
    {
        return await _userRepo
            .FindAsync(u => u.UsuRol == "CAJERO" && u.SedeId == sedeId && u.UsuEstado == "ACTIVO");
    }

    public async Task<List<Usuario>> GetCajerosPorSedeAsync(int sedeId)
    {
        return await _userRepo
            .FindAsync(u => u.UsuRol == "CAJERO" && u.SedeId == sedeId);
    }

    public async Task<int> GetUsuariosActivosCountAsync()
    {
        return await _userRepo.CountAsync(u =>
            (u.UsuRol == "CAJERO" || u.UsuRol == "ADMIN_LOCAL")
            && u.UsuEstado == "ACTIVO");
    }

    // Consultar todos los usuarios
    public async Task<List<Usuario>> GetAllUsersAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<bool> UpdateUserAsync(int id, UpdateUserRequest dto)
    {
        var usuarioExistente = await _context.Usuarios.FindAsync(id);
        if (usuarioExistente == null)
            return false;

        // Actualiza solo si viene con valor
        if (!string.IsNullOrWhiteSpace(dto.UsuPrimerNombre))
            usuarioExistente.UsuPrimerNombre = dto.UsuPrimerNombre;

        usuarioExistente.UsuSegundoNombre = string.IsNullOrWhiteSpace(dto.UsuSegundoNombre) ? null : dto.UsuSegundoNombre; // Puede ser nulo

        if (!string.IsNullOrWhiteSpace(dto.UsuPrimerApellido))
            usuarioExistente.UsuPrimerApellido = dto.UsuPrimerApellido;

        usuarioExistente.UsuSegundoApellido = string.IsNullOrWhiteSpace(dto.UsuSegundoApellido) ? null : dto.UsuSegundoApellido; // Puede ser nulo

        if (!string.IsNullOrWhiteSpace(dto.UsuCorreo))
            usuarioExistente.UsuCorreo = dto.UsuCorreo;

        if (!string.IsNullOrWhiteSpace(dto.UsuTelefono))
            usuarioExistente.UsuTelefono = dto.UsuTelefono;

        if (!string.IsNullOrWhiteSpace(dto.UsuEstado))
            usuarioExistente.UsuEstado = dto.UsuEstado.ToUpper();

        if (!string.IsNullOrWhiteSpace(dto.UsuEstado) &&
            dto.UsuEstado.ToUpper() != "ACTIVO" &&
            dto.UsuEstado.ToUpper() != "INACTIVO")
        {
            throw new ArgumentException("El estado debe ser 'ACTIVO' o 'INACTIVO'.");
        }


        await _userRepo.SaveChangesAsync();
        return true;
    }

    public async Task<List<Usuario>> GetUsersByRoleAsync(string rol)
    {
        return await _userRepo.FindAsync(u => u.UsuRol == rol);
    }

    public async Task<bool> DeactivateUserAsync(int id)
    {
        // Puedes usar el repositorio o directamente el contexto, según tu estándar
        var usuario = await _userRepo.GetByIdAsync(id);
        if (usuario == null)
            return false;

        // Si ya está inactivo, no pasa nada, pero consideramos la operación exitosa
        if (usuario.UsuEstado == "INACTIVO")
            return true;

        usuario.UsuEstado = "INACTIVO";

        await _userRepo.SaveChangesAsync();
        return true;
    }

}
