using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
// Alias QuestPDF colors to avoid ambiguity with System.Windows.Media.Colors
using QPdfColors = QuestPDF.Helpers.Colors;
using QuestPDF.Helpers;

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
                    page.Size(PageSizes.A4);
                    page.Margin(20);
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
                        columns.ConstantColumn(50); // Inventory ID
                        columns.RelativeColumn(2); // Item Name
                        columns.RelativeColumn(3); // Item Details
                        columns.RelativeColumn(2); // Category
                        columns.RelativeColumn(2); // SKU
                        columns.ConstantColumn(60); // Purchase
                        columns.ConstantColumn(60); // Sale
                        columns.ConstantColumn(40); // Qty
                        columns.ConstantColumn(40); // Unit
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellHeader).Text("Inv ID");
                        header.Cell().Element(CellHeader).Text("Item Name");
                        header.Cell().Element(CellHeader).Text("Details");
                        header.Cell().Element(CellHeader).Text("Category");
                        header.Cell().Element(CellHeader).Text("SKU");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Purchase");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Sale");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Qty");
                        header.Cell().Element(CellHeader).AlignCenter().Text("Unit");
                    });

                    foreach (var item in _items)
                    {
                        table.Cell().Element(CellData).Text(item?.itemInstance.InventoryId.ToString() ?? "0");
                        table.Cell().Element(CellData).Text(item?.ItemName ?? string.Empty);
                        table.Cell().Element(CellData).Text(item?.ItemDetails ?? string.Empty);
                        table.Cell().Element(CellData).Text(item?.Category ?? string.Empty);
                        table.Cell().Element(CellData).Text(item?.SKU ?? string.Empty);
                        table.Cell().Element(CellData).AlignCenter().Text(item?.PurchaseCost.ToString("0.00") ?? "0.00");
                        table.Cell().Element(CellData).AlignCenter().Text(item?.SalePrice.ToString("0.00") ?? "0.00");
                        table.Cell().Element(CellData).AlignCenter().Text(item?.Quantity.ToString() ?? "0");
                        table.Cell().Element(CellData).AlignCenter().Text(item?.Unit ?? string.Empty);
                    }
                });
            }

            IContainer CellHeader(IContainer container)
            {
                return container.DefaultTextStyle(x => x.SemiBold())
                    .Padding(5)
                    .Background(QPdfColors.Grey.Lighten4)
                    .BorderBottom(1)
                    .BorderColor(QPdfColors.Grey.Lighten2);
            }

            IContainer CellData(IContainer container)
            {
                return container.Padding(5)
                    .BorderBottom(1)
                    .BorderColor(QPdfColors.Grey.Lighten4);
            }
        }
    }
}
