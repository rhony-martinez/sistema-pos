using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SistemaPOS.Application.Common;

namespace SistemaPOS.Application.Sedes
{
    public class ListarSedesQuery
    {
        private readonly ISedeRepository _repo;
        public ListarSedesQuery(ISedeRepository repo) => _repo = repo;

        public async Task<Result<IEnumerable<SedeDto>>> ExecuteAsync()
        {
            var sedes = await _repo.ListarAsync();
            var dtos = sedes.Select(s => new SedeDto(
                s.SedeId, s.SedeNombre, s.SedeDireccion, s.SedeCiudad,
                s.SedeDepartamento, s.SedeUbicacion, s.SedeTelefono, s.SedeCorreo, s.SedeEstado));
            return Result<IEnumerable<SedeDto>>.Ok(dtos);
        }
    }
}
