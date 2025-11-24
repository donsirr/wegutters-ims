using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using WEGutters.UserClasses;
using static System.Net.Mime.MediaTypeNames;
// Alias QuestPDF colors to avoid ambiguity with System.Windows.Media.Colors
using QPdfColors = QuestPDF.Helpers.Colors;

namespace WEGutters
{
    public static class ExportToPDF
    {
        /// <summary>
        /// Shows a SaveFileDialog and exports the provided items to PDF if the user confirms.
        /// </summary>
        public static bool ExportWithSaveDialog(System.Windows.Window owner, IEnumerable<InventoryItemDisplay> items)
        {
            if (items == null || !items.Any())
                return false;

            var dlg = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                DefaultExt = "pdf",
                FileName = "StockReport.pdf"
            };

            var result = dlg.ShowDialog(owner);
            if (result != true)
                return false;

            Export(items, dlg.FileName);
            return true;
        }

        /// <summary>
        /// Exports the provided items to a PDF file at filePath. Throws on failure.
        /// </summary>
        public static void Export(IEnumerable<InventoryItemDisplay> items, string filePath)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Invalid file path.", nameof(filePath));

            // Ensure QuestPDF community license
            QuestPDF.Settings.License = LicenseType.Community;

            var list = items.ToList();
            var doc = new InventoryPdfDocument(list);
            doc.GeneratePdf(filePath);
        }

        //InventoryItemDisplay to PDF.
        private class InventoryPdfDocument : IDocument
        {
            private readonly List<InventoryItemDisplay> _items;

            public InventoryPdfDocument(List<InventoryItemDisplay> items)
            {
                _items = items ?? new List<InventoryItemDisplay>();
            }

            public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

            public void Compose(IDocumentContainer container)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(15);
                    page.PageColor(QPdfColors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(QPdfColors.Black));
                    page.Header().Height(50).AlignMiddle().Text("Stock Report").FontSize(18).SemiBold();
                    page.Content().PaddingVertical(5).Element(BuildTable);
                    page.Footer().AlignCenter().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}");
                });
            }

            void BuildTable(IContainer container)
            {
                container.Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30); // Inventory ID
                        columns.RelativeColumn(2); // Item Name
                        columns.RelativeColumn(2); // Item Details
                        columns.RelativeColumn(2); // Category
                        columns.RelativeColumn(2); // SKU
                        columns.ConstantColumn(80); // Purchase
                        columns.ConstantColumn(80); // Value
                        columns.ConstantColumn(80); // Sale
                        columns.ConstantColumn(80); // Projected
                        columns.ConstantColumn(40); // Qty
                        columns.ConstantColumn(40); // Min
                        columns.ConstantColumn(40); // Unit
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellHeader).Text("ID");
                        header.Cell().Element(CellHeader).Text("Name");
                        header.Cell().Element(CellHeader).Text("Details");
                        header.Cell().Element(CellHeader).Text("Category");
                        header.Cell().Element(CellHeader).Text("SKU");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Purchase");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Value");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Sale");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Projected");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Qty");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Min");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Unit");
                    });

                    foreach (var item in _items)
                    {
                        string bgColor = "#ffffff";
                        if (item.Quantity == item.MinQuantity)
                        {
                            bgColor = "#ffecad";
                        } else if (item.Quantity < item.MinQuantity)
                        {
                            bgColor = "#ff9696";
                        }
                            Cell(table, item?.itemInstance.InventoryId.ToString() ?? "0", bgColor);
                            Cell(table, item?.ItemName ?? "", bgColor);
                            Cell(table, item?.ItemDetails ?? "", bgColor);
                            Cell(table, item?.Category ?? "", bgColor);
                            Cell(table, item?.SKU ?? "", bgColor);
                            Cell(table, item?.PurchaseCost.ToString("$0.00") ?? "$0.00", bgColor, center: true);
                            Cell(table, item?.ItemValue.ToString("$0.00") ?? "$0.00", bgColor, center: true);
                            Cell(table, item?.SalePrice.ToString("$0.00") ?? "$0.00", bgColor, center: true);
                            Cell(table, item?.ProjectedSale.ToString("$0.00") ?? "$0.00", bgColor, center: true);
                            Cell(table, item?.Quantity.ToString() ?? "0", bgColor, center: true);
                            Cell(table, item?.MinQuantity.ToString() ?? "0", bgColor, center: true);
                            Cell(table, item?.Unit ?? "", bgColor, center: true);
                    }

                    int totalItems = _items.Count;
                    int belowMin = _items.Count(i => i.Quantity < i.MinQuantity);
                    int atMin = _items.Count(i => i.Quantity == i.MinQuantity);
                    float invValue = _items.Sum(i => i.itemInstance.calcItemValue());

                    table.Cell().ColumnSpan(4).Element(CellData).Element(e => e.Height(20)).Text($"Total Item Count: {totalItems}");
                    table.Cell().ColumnSpan(4).Element(CellData).Element(e => e.Height(20)).Text($"Items At Minimum: {atMin}");
                    table.Cell().ColumnSpan(4).Element(CellData).Element(e => e.Height(20)).Text($"Items Below Minimum: {belowMin}");
                    table.Cell().ColumnSpan(6).Element(CellData).Element(e => e.Height(20)).Text($"Inventory Value (Sum of all values): ${invValue:0.00}");
                    table.Cell().ColumnSpan(6).Element(CellData).Element(e => e.Height(20)).Text($"Projected Sales (Sum of all projected sales): ${invValue:0.00}");
                });
            }

            void BuildSummary(IContainer container)
            {
                int totalItems = _items.Count;
                int belowMin = _items.Count(i => i.Quantity < i.MinQuantity);
                int atMin = _items.Count(i => i.Quantity == i.MinQuantity);
                float invValue = _items.Sum(i => i.PurchaseCost);
                container.Table(table =>
                {
                    container.Padding(10).BorderTop(1).BorderColor(Colors.Grey.Lighten4).Column(col =>
                    {
                        col.Item().Text("Summary").Bold().FontSize(14);
                        col.Item().Text($"Total Inventory Value: {invValue:0.00}");
                        col.Item().Text($"Items at Minimum: {atMin}");
                        col.Item().Text($"Total Below Minimum: {belowMin}");
                    });
                    
                });

            }

            void Cell(TableDescriptor table, string text, string bgColor = "#ffffff", bool center = false)
            {
                table.Cell().Element(e =>
                {
                    e = e.Background(bgColor);
                    e = CellData(e);
                    if (center)
                        e = e.AlignCenter();
                    return e;
                }).Text(text);
            }

            IContainer CellHeader(IContainer container)
            {
                return container.DefaultTextStyle(x => x.SemiBold())
                    .Background(QPdfColors.Grey.Lighten4)
                    .BorderBottom(1)
                    .BorderColor(QPdfColors.Grey.Lighten2);
            }

            IContainer CellData(IContainer container)
            {
                return container.Padding(1)
                    .BorderBottom(1)
                    .BorderRight(1)
                    .BorderColor(QPdfColors.Grey.Lighten4);
            }
        }
    }
}
