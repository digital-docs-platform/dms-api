using Application.Listings;
using Application.UseCases.Queries.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries
{
    public interface IExportDocumentsListingQuery
                        : IProtectedUseCase,
                          IQuery<ExportDocumentsListingSearch, ExportFileResult>,
                          IAuditableUseCase<ExportDocumentsListingSearch>
    {
    }
}
