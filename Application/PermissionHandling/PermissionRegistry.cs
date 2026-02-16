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
                [PermissionCodes.DocumentVersionsRead] = PermissionScope.Document,
                [PermissionCodes.DocumentExport] = PermissionScope.Document,

                [PermissionCodes.DocumentTypesRead] = PermissionScope.DocumentType,

                [PermissionCodes.UsersRead] = PermissionScope.User,
                [PermissionCodes.UsersUpdate] = PermissionScope.User,

                [PermissionCodes.GroupsRead] = PermissionScope.Group,
                [PermissionCodes.GroupWrite] = PermissionScope.Group
            };

        // dependencies (Requires)
        public static readonly IReadOnlyDictionary<string, string[]> RequiresByCode =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                [PermissionCodes.DocumentsWrite] = new[] { PermissionCodes.DocumentsRead },
                [PermissionCodes.DocumentsDelete] = new[] { PermissionCodes.DocumentsRead },
                [PermissionCodes.DocumentExport] = new[] { PermissionCodes.DocumentsRead },
                [PermissionCodes.DocumentVersionsRead] = new[] { PermissionCodes.DocumentsRead },

                [PermissionCodes.UsersUpdate] = new[] { PermissionCodes.UsersRead },

                [PermissionCodes.GroupWrite] = new[] { PermissionCodes.GroupsRead }

          
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
