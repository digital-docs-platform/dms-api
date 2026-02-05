using Application.PermissionHandling.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Search
{
    public sealed class DocumentIdSearch : IHasDocumentId
    {
        public Guid DocumentId { get; set; }

    }
}
