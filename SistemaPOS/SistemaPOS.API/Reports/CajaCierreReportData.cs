namespace SistemaPOS.API.Reports
{
    public class CajaCierreReportData
    {
        public int CajaId { get; set; }
        public int SedeId { get; set; }
        public DateTime? FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }

        public decimal MontoInicial { get; set; }
        public decimal VentasNetas { get; set; }
        public decimal VentasEfectivo { get; set; }
        public decimal VentasTarjeta { get; set; }
        public decimal VentasTransferencia { get; set; }

        public int CantidadVentas { get; set; }
        public decimal TicketPromedio { get; set; }
        public decimal MontoFinal { get; set; }

        public List<VentaRow> Ventas { get; set; } = new();
        public List<ProductoRow> Productos { get; set; } = new();

        public class VentaRow
        {
            public int VenId { get; set; }
            public DateTime FechaVenta { get; set; }
            public string MetodoPago { get; set; } = "";
            public decimal Total { get; set; }
        }

        public class ProductoRow
        {
            public int ProId { get; set; }
            public string Nombre { get; set; } = "";
            public decimal Cantidad { get; set; }
            public decimal TotalVendido { get; set; }
        }
    }
}
