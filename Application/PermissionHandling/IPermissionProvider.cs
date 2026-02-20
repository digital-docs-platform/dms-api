using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PermissionHandling
{
    public interface IPermissionProvider
    {
        public Task<ICollection<UserPermissionsDto>> GetUserPermissionsAsync(
                                                     Guid uid,
                                                     CancellationToken ct = default);

        Task<ICollection<UserPermissionsDto>> GetUserPermissionGrantsAsync(
                                              Guid uid,
                                              CancellationToken ct = default);

        Task<ICollection<UserPermissionsDto>> GetGroupPermissionGrantsAsync(
                                              Guid gid,
                                              CancellationToken ct = default);
    }
}
