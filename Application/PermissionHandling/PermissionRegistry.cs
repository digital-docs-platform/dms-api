using Domain.Enums;

namespace Application.PermissionHandling
{
    public static class PermissionRegistry
    {
        public static readonly IReadOnlyDictionary<string, PermissionScope> ScopeByCode =
            new Dictionary<string, PermissionScope>(StringComparer.OrdinalIgnoreCase)
            {
                [PermissionCodes.SystemAdmin] = PermissionScope.System,

                [PermissionCodes.DocumentsRead] = PermissionScope.Document,
                [PermissionCodes.DocumentsWrite] = PermissionScope.Document,
                [PermissionCodes.DocumentsDelete] = PermissionScope.Document,
                [PermissionCodes.DocumentsVersionsRead] = PermissionScope.Document,
                [PermissionCodes.DocumentsExport] = PermissionScope.Document,
                [PermissionCodes.DocumentsDownload] = PermissionScope.Document,

                [PermissionCodes.UsersRead] = PermissionScope.User,
                [PermissionCodes.UsersWrite] = PermissionScope.User,
                [PermissionCodes.UsersUpdate] = PermissionScope.User,


                [PermissionCodes.GroupsRead] = PermissionScope.Group,
                [PermissionCodes.GroupWrite] = PermissionScope.Group,
                [PermissionCodes.GroupAddUser] = PermissionScope.Group,
                [PermissionCodes.GroupRemoveUser] = PermissionScope.Group
            };

        // dependencies (Requires)
        public static readonly IReadOnlyDictionary<string, string[]> RequiresByCode =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                [PermissionCodes.DocumentsWrite] = new[] { PermissionCodes.DocumentsRead },
                [PermissionCodes.DocumentsDelete] = new[] { PermissionCodes.DocumentsRead },
                [PermissionCodes.DocumentsExport] = new[] { PermissionCodes.DocumentsRead },
                [PermissionCodes.DocumentsVersionsRead] = new[] { PermissionCodes.DocumentsRead },
                [PermissionCodes.DocumentsDownload] = new[] {PermissionCodes.DocumentsRead},

                [PermissionCodes.UsersWrite] = new[] {PermissionCodes.UsersRead},
                [PermissionCodes.UsersUpdate] = new[] { PermissionCodes.UsersRead },
                

                [PermissionCodes.GroupWrite] = new[] { PermissionCodes.GroupsRead },
                [PermissionCodes.GroupAddUser] = new[] { PermissionCodes.GroupsRead },
                [PermissionCodes.GroupRemoveUser] = new[] { PermissionCodes.GroupsRead }


            };

        public static IReadOnlyList<string> GetAllCodes() =>
            ScopeByCode.Keys.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList();

        public static PermissionScope GetScope(string code) =>
            ScopeByCode.TryGetValue(code, out var scope)
                ? scope
                : throw new InvalidOperationException($"Permission code '{code}' is missing scope mapping in PermissionRegistry.");

        
        
        // helper za dobijanje svih dependency parova
        public static IEnumerable<(string permission, string dependsOn)> GetAllDependencies()
        {
            foreach (var (perm, deps) in RequiresByCode)
            {
                foreach (var dep in deps.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    yield return (perm, dep);
                }
            }
        }



    }
}
