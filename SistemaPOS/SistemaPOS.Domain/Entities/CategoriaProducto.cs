using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SistemaPOS.Domain.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("CATEGORIA_PRODUCTO")]
    public class CategoriaProducto
    {
        [Key]
        [Column("CAT_ID")]
        public int CatId { get; set; }

        [Required]
        [Column("CAT_NOMBRE")]
        [StringLength(50)]
        public string CatNombre { get; set; } = string.Empty;

        [Column("CAT_DESCRIPCION")]
        [StringLength(200)]
        public string? CatDescripcion { get; set; }

        [Column("CAT_ESTADO")]
        [StringLength(20)]
        public string? CatEstado { get; set; }

        // Relación con PRODUCTO
        public ICollection<Producto>? Productos { get; set; }
    }
}
