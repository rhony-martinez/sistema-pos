using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.Domain.Entities
{
    [Table("SEDE")]
    public class Sede
    {
        [Key]
        [Column("SEDE_ID")]
        public int SedeId { get; set; }

        [Required]
        [Column("SEDE_NOMBRE")]
        [StringLength(100)]
        public string SedeNombre { get; set; } = string.Empty;

        [Column("SEDE_DIRECCION")]
        [StringLength(150)]
        public string? SedeDireccion { get; set; }

        [Column("SEDE_CIUDAD")]
        [StringLength(80)]
        public string? SedeCiudad { get; set; }

        [Column("SEDE_DEPARTAMENTO")]
        [StringLength(80)]
        public string? SedeDepartamento { get; set; }

        [Column("SEDE_UBICACION")]
        [StringLength(100)]
        public string? SedeUbicacion { get; set; }

        [Column("SEDE_TELEFONO")]
        [StringLength(20)]
        public string? SedeTelefono { get; set; }

        [Column("SEDE_CORREO")]
        [StringLength(100)]
        public string? SedeCorreo { get; set; }

        [Column("SEDE_ESTADO")]
        [StringLength(20)]
        public string? SedeEstado { get; set; }

        public ICollection<Caja>? Cajas { get; set; }
        public ICollection<Usuario>? Usuarios { get; set; }
        public ICollection<Catalogo>? Catalogos { get; set; }
    }
}
