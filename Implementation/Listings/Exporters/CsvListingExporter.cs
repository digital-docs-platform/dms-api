using System.Text;
using Application.Listings;

namespace Implementation.Listings.Exporters
{
    public sealed class CsvListingExporter : IListingExporter
    {
        public ListingExportFormat Format => ListingExportFormat.Csv;

        public Task<ExportFileResult> ExportAsync(ListingDocument document, CancellationToken ct)
        {
            var sb = new StringBuilder();

            // Header
            sb.AppendLine(string.Join(",", document.Headers.Select(Escape)));

            // Rows
            foreach (var row in document.Rows)
            {
                var cells = new string[document.Headers.Count];

                for (int i = 0; i < document.Headers.Count; i++)
                {
                    var value = i < row.Count ? row[i] : "";
                    cells[i] = Escape(value ?? "");
                }

                sb.AppendLine(string.Join(";", cells));
            }

            // Excel u Srbiji često očekuje ; (semicolon) zbog decimal separator-a.
            // Ako ti treba ; umesto , samo zameni join separator.

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            return Task.FromResult(new ExportFileResult
            {
                Content = bytes,
                ContentType = "text/csv; charset=utf-8",
                FileName = "listing.csv"
            });
        }

        private static string Escape(string s)
        {
            // CSV pravilo: ako sadrži , " \n ili \r → ide u navodnike, a " se duplira
            var mustQuote = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
            if (!mustQuote) return s;

            return $"\"{s.Replace("\"", "\"\"")}\"";
        }
    }
}
