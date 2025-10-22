using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SistemaPOS.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("DETALLE_VENTA")]
    public class DetalleVenta
    {
        [Key]
        [Column("DET_ID")]
        public int DetId { get; set; }

        [Column("VEN_ID")]
        public int VenId { get; set; }

        [ForeignKey("VenId")]
        public Venta? Venta { get; set; }

        [Column("PRO_ID")]
        public int ProId { get; set; }

        [ForeignKey("ProId")]
        public Producto? Producto { get; set; }

        [Column("DET_CANTIDAD")]
        public decimal DetCantidad { get; set; }

        [Column("DET_PRECIO_UNITARIO")]
        public decimal DetPrecioUnitario { get; set; }

        [NotMapped] // No se persiste, solo se calcula
        public decimal DetSubtotal => DetCantidad * DetPrecioUnitario;
    }
}
