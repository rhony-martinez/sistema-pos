using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.Domain.Entities
{
    [Table("CAJA")]
    public class Caja
    {
        [Key]
        [Column("CAJA_ID")]
        public int CajaId { get; set; }

        [Column("CAJA_FECHA_APERTURA")]
        public DateTime? CajaFechaApertura { get; set; }

        [Column("CAJA_FECHA_CIERRE")]
        public DateTime? CajaFechaCierre { get; set; }

        [Column("CAJA_MONTO_INICIAL")]
        public decimal? CajaMontoInicial { get; set; }

        [Column("CAJA_MONTO_FINAL")]
        public decimal? CajaMontoFinal { get; set; }

        [Required]
        [Column("CAJA_ESTADO")]
        [StringLength(20)]
        public string CajaEstado { get; set; } = string.Empty;

        [Column("SEDE_ID")]
        public int SedeId { get; set; }

        [ForeignKey("SedeId")]
        public Sede? Sede { get; set; }

        //[Column("USU_ID")]
        //public int UsuarioId { get; set; }

        //[ForeignKey("UsuarioId")]
        //public Usuario? Usuario { get; set; }
    }
}