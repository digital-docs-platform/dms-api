using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PermissionHandling
{
    public static class PermissionCodes
    {
        public const string SystemAdmin = "system.admin";

        public const string DocumentsRead = "documents.read";
        public const string DocumentsWrite = "documents.write";
        public const string DocumentsDelete = "documents.delete";
        public const string DocumentsVersionsRead = "document.versions.read";
        public const string DocumentsExport = "document.export";
        public const string DocumentsDownload = "documents.download";

        public const string UsersRead = "users.read";
        public const string UsersWrite = "users.write";
        public const string UsersUpdate = "users.update";
        public const string UsersPermissionGrant = "users.permission.grant";

        public const string GroupsRead = "groups.read";
        public const string GroupWrite = "groups.write";
    }
}
