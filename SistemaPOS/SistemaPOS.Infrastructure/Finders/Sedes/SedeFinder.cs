using Microsoft.EntityFrameworkCore;
using SistemaPOS.Application.Queries.Sedes;
using SistemaPOS.Application.Queries.Usuarios;
using SistemaPOS.Infrastructure.Data;

namespace SistemaPOS.Infrastructure.Finders.Sedes
{
    public sealed class SedeFinder : ISedeQueries
    {
        private readonly SistemaPosContext _ctx;
        public SedeFinder(SistemaPosContext ctx) => _ctx = ctx;

        public async Task<IReadOnlyList<SedeDto>> GetAllAsync()
        {
            return await _ctx.Sedes
                .AsNoTracking()
                .OrderBy(s => s.SedeId)
                .Select(s => new SedeDto(
                    s.SedeId,
                    s.SedeNombre,
                    s.SedeDireccion ?? string.Empty,
                    s.SedeTelefono ?? string.Empty,
                    s.SedeEstado ?? string.Empty,
                    s.Usuarios
                     .Select(u => new UsuarioDto(
                         u.UsuId,
                         (u.UsuPrimerNombre ?? string.Empty) + " " + (u.UsuPrimerApellido ?? string.Empty),
                         u.UsuUsername,
                         u.UsuRol,
                         u.UsuEstado
                     ))
                     .ToList()
                ))
                .ToListAsync();
        }

    }
}
