using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaPOS.Application.Sedes
{
    public record SedeDto(
        long SedeId,
        string Nombre,
        string? Direccion,
        string? Ciudad,
        string? Departamento,
        string? Ubicacion,
        string? Telefono,
        string? Correo,
        string Estado
    )
    {
        public string CodigoUi => SedeId > 0 ? $"SN{SedeId}" : "SN";
        public string CodigoTablaUi => SedeId > 0 ? $"#S-{SedeId:D3}" : "";
    };
}
