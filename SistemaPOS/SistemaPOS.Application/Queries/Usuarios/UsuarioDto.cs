namespace SistemaPOS.Application.Queries.Usuarios
{
    public sealed record UsuarioDto(int UsuId, string NombreCompleto, string Username, string? Rol, string? Estado);
}
