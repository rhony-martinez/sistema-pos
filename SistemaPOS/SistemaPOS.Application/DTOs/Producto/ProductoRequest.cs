using System.ComponentModel.DataAnnotations;

namespace SistemaPOS.Application.DTOs.Producto;

public class ProductoRequest
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(100)]
    public string ProNombre { get; set; } = string.Empty;

    [StringLength(200)]
    public string? ProDescripcion { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal ProPrecioVenta { get; set; }

    [StringLength(20)]
    public string? ProUnidad { get; set; }

    [Required(ErrorMessage = "Debe especificar el nombre de la categoría.")]
    public string CatNombre { get; set; } = string.Empty;
}

