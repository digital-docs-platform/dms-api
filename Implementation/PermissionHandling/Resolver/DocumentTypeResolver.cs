using Application.PermissionHandling.Resolver;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.PermissionHandling.Resolver
{
    public class DocumentTypeResolver : IDocumentTypeResolver
    {
        private readonly DatabaseContext _db;
        public DocumentTypeResolver(DatabaseContext db) 
        {
            _db = db;
        }
        public async Task<int> ResolveAsync(object request, CancellationToken ct)
        {
            if(request is IHasDocumentTypeId hasType)
                return hasType.DocumentTypeId;

            if(request is IHasDocumentId hasDoc)
                return await _db.Documents.Where(d => d.Id == hasDoc.DocumentId)
                                          .Select(x => x.DocumentTypeId)
                                          .FirstOrDefaultAsync(ct);


            throw new InvalidOperationException($"Request '{request.GetType().Name}' cannot resolve DocumentType scope (missing DocumentTypeId or DocumentId).");
        }
    }
}
