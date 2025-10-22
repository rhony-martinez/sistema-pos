using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SistemaPOS.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("VENTA")]
    public class Venta
    {
        [Key]
        [Column("VEN_ID")]
        public int VenId { get; set; }

        [Column("FECHA_VENTA")]
        public DateTime FechaVenta { get; set; } = DateTime.Now;

        [Column("VEN_TOTAL")]
        public decimal? VenTotal { get; set; }

        [Column("VEN_METODO_PAGO")]
        [StringLength(30)]
        public string? VenMetodoPago { get; set; }

        [Column("CAJA_ID")]
        public int CajaId { get; set; }

        [ForeignKey("CajaId")]
        public Caja? Caja { get; set; }

        public ICollection<DetalleVenta>? Detalles { get; set; }
    }
}
