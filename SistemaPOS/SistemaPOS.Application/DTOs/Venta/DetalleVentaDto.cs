using System.ComponentModel.DataAnnotations;

namespace SistemaPOS.Application.DTOs.Venta
{
    public class DetalleVentaDto
    {
        [Required]
        public int ProId { get; set; }

        [Required]
        public decimal DetCantidad { get; set; }

        [Required]
        public decimal DetPrecioUnitario { get; set; }
    }
}
