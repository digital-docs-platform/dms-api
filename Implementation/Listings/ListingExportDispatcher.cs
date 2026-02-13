using Application.Listings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.Listings
{
    public sealed class ListingExportDispatcher : IListingExportDispatcher
    {
        private readonly IReadOnlyDictionary<ListingExportFormat, IListingExporter> _map;

        public ListingExportDispatcher(IEnumerable<IListingExporter> exporters)
        {
            _map  = exporters.ToDictionary(e => e.Format, e => e);
        }
        public Task<ExportFileResult> ExecuteAsync(ListingExportFormat format, ListingDocument document, CancellationToken ct)
        {
            if (!_map.TryGetValue(format, out var exporter))
                throw new UnsupportedExportFormatException($"Unsupported export format: {format}");

            return exporter.ExportAsync(document, ct);
        }
    }
}
