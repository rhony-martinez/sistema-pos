using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SistemaPOS.Domain.Entities

{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public required string Nombre { get; set; }
        // ...
    }
}

