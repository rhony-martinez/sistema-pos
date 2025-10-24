namespace SistemaPOS.Application.DTOs
{
    public class UpdateUserRequest
    {
        // Datos personales
        public string UsuPrimerNombre { get; set; } = string.Empty;
        public string? UsuSegundoNombre { get; set; }
        public string UsuPrimerApellido { get; set; } = string.Empty;
        public string? UsuSegundoApellido { get; set; }

        // Contacto
        public string UsuCorreo { get; set; } = string.Empty;
        public string UsuTelefono { get; set; } = string.Empty;

        // Estado (ACTIVO / INACTIVO)
        public string UsuEstado { get; set; } = "ACTIVO";
    }
}
