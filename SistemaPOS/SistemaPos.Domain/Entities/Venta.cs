using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaPOS.Domain.Entities
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total { get; set; }
    }
}

