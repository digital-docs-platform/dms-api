using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PermissionHandling
{
    public interface IPermissionHandler
    {
        public Task EnsureAsync(string permissionCode, Guid? documentTypeId, CancellationToken ct = default);
        //public void EnsureAll(IEnumerable<string> permissions);
        //public bool Has(string permission);
        //public bool HasAll(IEnumerable<string> permissions);
    }
}
