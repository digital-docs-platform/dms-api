using Application.Listings;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.Listings.Exporters
{
    public sealed class ExcelListingExporter : IListingExporter
    {
        public ListingExportFormat Format => ListingExportFormat.Excel;

        public Task<ExportFileResult> ExportAsync(ListingDocument document, CancellationToken ct)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Listing");

            // Title (opciono)
            if (!string.IsNullOrWhiteSpace(document.Title))
            {
                ws.Cell(1, 1).Value = document.Title;
                ws.Range(1, 1, 1, document.Headers.Count).Merge();
                ws.Cell(1, 1).Style.Font.Bold = true;
                ws.Cell(1, 1).Style.Font.FontSize = 14;
            }

            var headerRowIndex = string.IsNullOrWhiteSpace(document.Title) ? 1 : 3;
            var startDataRow = headerRowIndex + 1;

            // Headers
            for (int c = 0; c < document.Headers.Count; c++)
            {
                ws.Cell(headerRowIndex, c + 1).Value = document.Headers[c] ?? "";
                ws.Cell(headerRowIndex, c + 1).Style.Font.Bold = true;
            }

            // Rows
            for (int r = 0; r < document.Rows.Count; r++)
            {
                var row = document.Rows[r];
                for (int c = 0; c < document.Headers.Count; c++)
                {
                    var value = c < row.Count ? row[c] : "";
                    ws.Cell(startDataRow + r, c + 1).Value = value ?? "";
                }
            }

            // Basic formatting
            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);

            return Task.FromResult(new ExportFileResult
            {
                Content = ms.ToArray(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = "listing.xlsx"
            });
        }
    }
}
