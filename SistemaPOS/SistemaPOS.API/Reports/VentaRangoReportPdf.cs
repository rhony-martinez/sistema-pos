using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SistemaPOS.API.Reports
{
    public class VentasRangoReportPdf : IDocument
    {
        private readonly VentasRangoReportData _d;
        public VentasRangoReportPdf(VentasRangoReportData data) => _d = data;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(25);
                page.Size(PageSizes.A4);

                page.Header().Column(col =>
                {
                    col.Item().Text("Informe de Ventas").FontSize(18).SemiBold();
                    col.Item().Text($"Rango: {_d.Desde:yyyy-MM-dd} a {_d.Hasta:yyyy-MM-dd}")
                      .FontSize(10).FontColor(Colors.Grey.Darken2);
                });

                page.Content().Column(col =>
                {
                    col.Spacing(12);

                    col.Item().Element(SummaryCard);
                    col.Item().Element(BarChartMetodos);

                    col.Item().Text("Detalle de ventas").FontSize(13).SemiBold();
                    col.Item().Element(VentasTable);

                    col.Item().Text("Resumen por producto").FontSize(13).SemiBold();
                    col.Item().Element(ProductosTable);
                });

                page.Footer().AlignRight().Text($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(9);
            });
        }

        void SummaryCard(IContainer c)
        {
            c.Padding(10).Background(Colors.Grey.Lighten4).CornerRadius(6).Column(col =>
            {
                col.Spacing(4);
                col.Item().Text($"Ventas netas: {_d.VentasNetas:n0}");
                col.Item().Text($"# ventas: {_d.CantidadVentas}");
                col.Item().Text($"Ticket promedio: {_d.TicketPromedio:n0}");
            });
        }

        void BarChartMetodos(IContainer c)
        {
            var values = new List<(string label, decimal value)>
        {
          ("Efectivo", _d.VentasEfectivo),
          ("Tarjeta", _d.VentasTarjeta),
          ("Transfer.", _d.VentasTransferencia)
        };

            var max = values.Max(v => v.value);
            if (max <= 0) max = 1;

            c.Padding(10).Border(1).BorderColor(Colors.Grey.Lighten2).CornerRadius(6).Column(col =>
            {
                col.Item().Text("Ventas por método de pago").SemiBold();

                foreach (var (label, value) in values)
                {
                    col.Item().Row(row =>
                    {
                        row.ConstantColumn(70).Text(label).FontSize(10);

                        row.RelativeColumn().Height(12).Background(Colors.Grey.Lighten3).AlignMiddle().Element(bar =>
                        {
                            bar.Row(r =>
                            {
                                var pct = (float)(value / max);
                                r.RelativeColumn(pct * 100).Height(12).Background(Colors.Blue.Lighten2);
                                r.RelativeColumn(100 - pct * 100);
                            });
                        });

                        row.ConstantColumn(80).AlignRight().Text($"{value:n0}").FontSize(10);
                    });
                }
            });
        }

        void VentasTable(IContainer c)
        {
            c.Table(t =>
            {
                t.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(60);  // ID
                    cols.ConstantColumn(90);  // Fecha
                    cols.RelativeColumn();    // Método
                    cols.ConstantColumn(100); // Total
                });

                t.Header(h =>
                {
                    h.Cell().Text("VenId").SemiBold();
                    h.Cell().Text("Fecha").SemiBold();
                    h.Cell().Text("Método").SemiBold();
                    h.Cell().AlignRight().Text("Total").SemiBold();
                    h.Cell().ColumnSpan(4).PaddingTop(2).LineHorizontal(1);
                });

                foreach (var v in _d.Ventas)
                {
                    t.Cell().Text(v.VenId.ToString());
                    t.Cell().Text(v.FechaVenta.ToString("yyyy-MM-dd HH:mm"));
                    t.Cell().Text(v.MetodoPago);
                    t.Cell().AlignRight().Text($"{v.Total:n0}");
                }
            });
        }

        void ProductosTable(IContainer c)
        {
            c.Table(t =>
            {
                t.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(60);
                    cols.RelativeColumn();
                    cols.ConstantColumn(80);
                    cols.ConstantColumn(110);
                });

                t.Header(h =>
                {
                    h.Cell().Text("ProId").SemiBold();
                    h.Cell().Text("Producto").SemiBold();
                    h.Cell().AlignRight().Text("Cant.").SemiBold();
                    h.Cell().AlignRight().Text("Total").SemiBold();
                    h.Cell().ColumnSpan(4).PaddingTop(2).LineHorizontal(1);
                });

                foreach (var p in _d.Productos.OrderByDescending(x => x.TotalVendido))
                {
                    t.Cell().Text(p.ProId.ToString());
                    t.Cell().Text(p.Nombre);
                    t.Cell().AlignRight().Text($"{p.Cantidad:n0}");
                    t.Cell().AlignRight().Text($"{p.TotalVendido:n0}");
                }
            });
        }
    }
}
