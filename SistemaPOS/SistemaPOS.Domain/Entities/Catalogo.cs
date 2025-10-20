using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaPOS.Domain.Entities
{
    [Table("CATALOGO")]
    public class Catalogo
    {
        [Column("SEDE_ID")]
        public int SedeId { get; set; }

        [ForeignKey("SedeId")]
        public Sede? Sede { get; set; }

        [Column("PRO_ID")]
        public int ProId { get; set; }

        [ForeignKey("ProId")]
        public Producto? Producto { get; set; }
    }
}
