using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace SistemaPOS.Domain.Entities
{
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

        // Relación con PRODUCTO
        [JsonIgnore]
        public ICollection<Producto>? Productos { get; set; }

    }
}
