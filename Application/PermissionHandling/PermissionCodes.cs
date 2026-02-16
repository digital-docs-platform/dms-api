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
        public const string DocumentVersionsRead = "document.versions.read";
        public const string DocumentExport = "document.export";

        public const string DocumentTypesRead = "documentType.read";

        public const string UsersRead = "users.read";
        public const string UsersUpdate = "users.update";

        public const string GroupsRead = "groups.read";
        public const string GroupWrite = "groups.write";
    }
}
