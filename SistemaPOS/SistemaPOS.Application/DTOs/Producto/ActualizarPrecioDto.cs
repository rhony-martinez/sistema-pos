using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SistemaPOS.Application.DTOs.Producto
{
    public class ActualizarPrecioDto
    {
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal ProPrecioVenta { get; set; }
    }
}
