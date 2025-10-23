using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.Domain.Entities
{
    [Table("USUARIO")]
    public class Usuario
    {
        [Key]
        [Column("USU_ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int UsuId { get; set; }

        [Required]
        [Column("USU_PRIMER_NOMBRE")]
        [StringLength(50)]
        public string UsuPrimerNombre { get; set; } = string.Empty;

        [Column("USU_SEGUNDO_NOMBRE")]
        [StringLength(50)]
        public string? UsuSegundoNombre { get; set; }

        [Required]
        [Column("USU_PRIMER_APELLIDO")]
        [StringLength(50)]
        public string UsuPrimerApellido { get; set; } = string.Empty;

        [Column("USU_SEGUNDO_APELLIDO")]
        [StringLength(50)]
        public string? UsuSegundoApellido { get; set; }

        [Required]
        [Column("USU_CORREO")]
        [StringLength(100)]
        public string UsuCorreo { get; set; } = string.Empty;

        [Column("USU_TELEFONO")]
        [StringLength(20)]
        public string? UsuTelefono { get; set; }

        [Required]
        [Column("USU_USERNAME")]
        [StringLength(50)]
        public string UsuUsername { get; set; } = string.Empty;

        [Required]
        [Column("USU_CLAVE_HASH")]
        [StringLength(255)]
        public string UsuClaveHash { get; set; } = string.Empty;

        [Required]
        [Column("USU_ESTADO")]
        [StringLength(20)]
        public string UsuEstado { get; set; } = string.Empty;

        [Column("USU_ROL")]
        [StringLength(30)]
        public string? UsuRol { get; set; }

        [Column("SEDE_ID")]
        public int? SedeId { get; set; }

        [ForeignKey("SedeId")]
        public Sede? Sede { get; set; }
    }
}