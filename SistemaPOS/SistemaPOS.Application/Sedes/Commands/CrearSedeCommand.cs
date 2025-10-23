using System.Text.RegularExpressions;
using SistemaPOS.Application.Common;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.Application.Sedes
{
    public enum CrearSedeError { Ninguno, Duplicada, DatosInvalidos, Desconocido }

    public class CrearSedeCommand
    {
        private readonly ISedeRepository _repo;
        public CrearSedeCommand(ISedeRepository repo) => _repo = repo;

        public async Task<Result<long>> ExecuteAsync(
            string nombre, string? direccion, string? ciudad, string? departamento,
            string? ubicacion, string? telefono, string? correo, string? estado = "ACTIVA")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(correo) && !Regex.IsMatch(correo, @"^\S+@\S+\.\S+$"))
                    return Result<long>.Fail(CrearSedeError.DatosInvalidos.ToString());

                if (!string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(ciudad))
                {
                    if (await _repo.ExisteDuplicadaAsync(nombre, ciudad))
                        return Result<long>.Fail(CrearSedeError.Duplicada.ToString());
                }

                var entidad = new Sede(nombre, direccion, ciudad, departamento, ubicacion, telefono, correo, estado);
                var id = await _repo.CrearAsync(entidad);
                entidad.SetId(id);
                return Result<long>.Ok(id);
            }
            catch
            {
                return Result<long>.Fail(CrearSedeError.Desconocido.ToString());
            }
        }
    }
}