using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Listings
{
    public interface IListingExporter
    {
        public ListingExportFormat Format { get; }
        public Task<ExportFileResult> ExportAsync(ListingDocument document, CancellationToken ct);
    }
}
