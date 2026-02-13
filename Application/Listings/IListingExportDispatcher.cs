using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Listings
{
    public interface IListingExportDispatcher
    {
        public Task<ExportFileResult> ExecuteAsync(ListingExportFormat format, ListingDocument document, CancellationToken ct);
    }
}
