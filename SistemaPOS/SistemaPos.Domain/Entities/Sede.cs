using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.Domain.Entities
{
    [Table("SEDE")]
    public class Sede
    {
        [Key]
        [Column("SEDE_ID")]
        public int SEDE_ID { get; set; }

        [Column("SEDE_NOMBRE")]
        public string SEDE_NOMBRE { get; set; } = string.Empty;

        [Column("SEDE_CIUDAD")]
        public string? SEDE_CIUDAD { get; set; }

        [Column("SEDE_DEPARTAMENTO")]
        public string? SEDEE_DEPARTAMENTO { get; set; }

        [Column("SEDE_UBICACION")]
        public string? SEDE_UBICACION { get; set; }

        [Column("SEDE_CORREO")]
        public string? SEDE_CORREO { get; set; }

        [Column("SEDE_TELEFONO")]
        public string? SEDE_TELEFONO { get; set; }

        [Column("SEDE_ESTADO")]
        public string? SEDE_ESTADO { get; set; }
    }
}
