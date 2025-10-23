using System.Text.RegularExpressions;
using SistemaPOS.Application.Common;
using SistemaPOS.Domain.Entities;

using SistemaPOS.Application.Sedes;


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
                // Validar correo
                if (!string.IsNullOrWhiteSpace(correo) && !Regex.IsMatch(correo, @"^\S+@\S+\.\S+$"))
                    return Result<long>.Fail(CrearSedeError.DatosInvalidos.ToString());

                // Validar duplicado
                if (!string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(ciudad))
                {
                    if (await _repo.BuscarSedeAsync(null, nombre) is not null)
                        return Result<long>.Fail(CrearSedeError.Duplicada.ToString());
                }

                // Crear entidad (usando inicializador, no constructor)
                var entidad = new Sede
                {
                    SedeNombre = nombre,
                    SedeDireccion = direccion,
                    SedeCiudad = ciudad,
                    SedeDepartamento = departamento,
                    SedeUbicacion = ubicacion,
                    SedeTelefono = telefono,
                    SedeCorreo = correo,
                    SedeEstado = estado ?? "ACTIVA"
                };

                // Insertar en el repositorio
                await _repo.AddAsync(entidad);

                // Retornar el ID generado
                return Result<long>.Ok(entidad.SedeId);
            }
            catch
            {
                return Result<long>.Fail(CrearSedeError.Desconocido.ToString());
            }
        }
    }
}
