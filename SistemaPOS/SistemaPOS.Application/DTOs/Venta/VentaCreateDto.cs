using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaPOS.Application.DTOs.Venta
{
    public class VentaCreateDto
    {
        [Required]
        public string VenMetodoPago { get; set; } = string.Empty;

        [Required]
        public int CajaId { get; set; }

        [Required]
        public List<DetalleVentaDto> Detalles { get; set; } = new();
    }
}