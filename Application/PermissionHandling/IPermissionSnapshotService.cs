using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Application.PermissionHandling
{
    public interface IPermissionSnapshotService
    {
        
            // TGrant is any existing grant entity type
            (List<TGrant> toRemove, List<PermissionKey> toAdd) Reconcile<TGrant>(
                IReadOnlyCollection<TGrant> current,
                IEnumerable<PermissionKey> requested,
                Func<TGrant, int> getPermissionId,
                Func<TGrant, Guid?> getDocumentTypeId);
        
    }

    public sealed class PermissionKey
    {
        public int PermissionId {  get; set; }
        public Guid? DocumentTypeId { get; set; }
    }
}
