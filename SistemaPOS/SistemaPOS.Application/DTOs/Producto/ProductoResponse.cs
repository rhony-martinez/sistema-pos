namespace SistemaPOS.Application.DTOs.Producto
{
    public class ProductoResponse
    {
        public int ProId { get; set; }
        public string ProNombre { get; set; } = string.Empty;
        public string? ProDescripcion { get; set; }
        public decimal ProPrecioVenta { get; set; }
        public string? ProUnidad { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public int SedeId { get; set; }
    }
}
