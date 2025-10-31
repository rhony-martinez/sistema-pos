using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaPOS.Application.DTOs.Producto
{
    public class ProductoResponse
    {
        public int ProId { get; set; }
        public string ProNombre { get; set; } = string.Empty;
        public string? ProDescripcion { get; set; }
        public decimal ProPrecioVenta { get; set; }
        public string? ProUnidad { get; set; }
        public string CatNombre { get; set; } = string.Empty;
    }
}
