using SistemaPOS.Application.Queries.Usuarios;

namespace SistemaPOS.Application.Queries.Sedes
{
    public sealed record SedeDto(int SedeId, string SedeNombre, string SedeDireccion, string SedeTelefono, string SedeEstado, IReadOnlyList<UsuarioDto> Usuarios
);
}
