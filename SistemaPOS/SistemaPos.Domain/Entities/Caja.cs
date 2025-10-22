using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SistemaPOS.Domain.Entities
{
    public class Caja
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
    }
}

