using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PermissionHandling.Resolver
{
    public interface IDocumentTypeResolver
    {
        public Task<int> ResolveAsync(object request, CancellationToken ct);
    }
}
