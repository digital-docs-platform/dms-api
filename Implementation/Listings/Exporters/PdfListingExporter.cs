using Application.Listings;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.Listings.Exporters
{
    public sealed class PdfListingExporter : IListingExporter
    {
        public ListingExportFormat Format => ListingExportFormat.Pdf;

        public Task<ExportFileResult> ExportAsync(ListingDocument document,CancellationToken ct)
        {

            var bytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4.Landscape());

                    page.Header().Text(document.Title).SemiBold().FontSize(16);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            for (int i = 0; i < document.Headers.Count; i++)
                                cols.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            foreach (var h in document.Headers)
                                header.Cell().Padding(4).Text(h ?? "").SemiBold();
                        });

                        foreach (var row in document.Rows)
                        {
                            for (int i = 0; i < document.Headers.Count; i++)
                            {
                                var value = (i < row.Count ? row[i] : "") ?? "";
                                table.Cell().Padding(4).Text(value);
                            }
                        }
                    });

                    page.Footer().AlignRight().Text($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm}");
                });
            }).GeneratePdf();

            return Task.FromResult(new ExportFileResult
            {
                Content = bytes,
                ContentType = "application/pdf",
                FileName = "listing.pdf"
            });
        }
    }
}
