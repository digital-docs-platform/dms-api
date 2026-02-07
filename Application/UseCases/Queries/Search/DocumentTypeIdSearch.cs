using Application.PermissionHandling.Resolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Search
{
    public class DocumentTypeIdSearch : IHasDocumentTypeId
    {
        public Guid DocumentTypeId { get; set; }
    }
}
