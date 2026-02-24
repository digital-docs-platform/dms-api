using Application.PermissionHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.PermissionHandling
{
    public sealed class PermissionSnapshotService : IPermissionSnapshotService
    {
        public (List<TGrant> toRemove, List<PermissionKey> toAdd) Reconcile<TGrant>(IReadOnlyCollection<TGrant> current, IEnumerable<PermissionKey> requested, Func<TGrant, int> getPermissionId, Func<TGrant, Guid?> getDocumentTypeId)
        {
            var requestedSet = new HashSet<(int, Guid?)>(requested.Select(r => (r.PermissionId, r.DocumentTypeId)));
            var currentSet = new HashSet<(int, Guid?)>(current.Select(c => (getPermissionId(c), getDocumentTypeId(c))));

            var toRemove = current.Where(c => !requestedSet.Contains((getPermissionId(c), getDocumentTypeId(c)))).ToList();
            var toAdd = requested.Where(r => !currentSet.Contains((r.PermissionId, r.DocumentTypeId))).ToList();

            return (toRemove, toAdd);
        }
    }
}
