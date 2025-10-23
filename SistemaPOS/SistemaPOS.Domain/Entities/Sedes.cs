using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaPOS.Domain.Entities
{
    public class Sede
    {
        public long SedeId { get; private set; }
        public string SedeNombre { get; private set; } = string.Empty;
        public string? SedeDireccion { get; private set; }
        public string? SedeCiudad { get; private set; }
        public string? SedeDepartamento { get; private set; }
        public string? SedeUbicacion { get; private set; }
        public string? SedeTelefono { get; private set; }
        public string? SedeCorreo { get; private set; }
        public string SedeEstado { get; private set; } = "ACTIVA";

        public Sede(string nombre, string? direccion, string? ciudad, string? departamento,
                    string? ubicacion, string? telefono, string? correo, string? estado = "ACTIVA")
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio");
            SedeNombre = nombre.Trim();
            SedeDireccion = direccion?.Trim();
            SedeCiudad = ciudad?.Trim();
            SedeDepartamento = departamento?.Trim();
            SedeUbicacion = ubicacion?.Trim();
            SedeTelefono = telefono?.Trim();
            SedeCorreo = correo?.Trim();
            SedeEstado = string.IsNullOrWhiteSpace(estado) ? "ACTIVA" : estado!;
        }

        public void SetId(long id) => SedeId = id;
    }
}