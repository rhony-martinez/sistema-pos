using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.Domain.Entities
{
    [Table("PRODUCTO")]
    public class Producto
    {
        [Key]
        [Column("PRO_ID")]
        public int ProId { get; set; }

        [Required]
        [Column("PRO_NOMBRE")]
        [StringLength(100)]
        public string ProNombre { get; set; } = string.Empty;

        [Column("PRO_DESCRIPCION")]
        [StringLength(200)]
        public string? ProDescripcion { get; set; }

        [Column("PRO_PRECIO_VENTA")]
        public decimal ProPrecioVenta { get; set; }

        [Column("PRO_UNIDAD")]
        [StringLength(20)]
        public string? ProUnidad { get; set; }

        [Column("PRO_ESTADO")]
        [StringLength(20)]
        public string? ProEstado { get; set; }

        [Column("CAT_ID")]
        public int CatId { get; set; }

        [ForeignKey("CatId")]
        public CategoriaProducto? Categoria { get; set; }
    }
}
