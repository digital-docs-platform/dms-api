using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Response
{
    public sealed class GetPermissionCatalogResponse
    {
        public List<PermissionScopeGroupDto> Scopes { get; set; } = [];
        public List<UserGrantedPermissionDto> UserPermissions { get; set; } = [];
        public List<DocumentTypesDto> DocumentTypes { get; set; } = [];
    }

    public sealed class PermissionScopeGroupDto
    {
        public PermissionScope Scope { get; set; }
        public List<PermissionItemDto> Permissions { get; set; } = new();
    }

    public sealed class PermissionItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        
        public List<string> DependsOn { get; set; } = new();
    }

    public sealed class UserGrantedPermissionDto
    {
        public string Code { get; set; } = null!;
        public Guid? DocumentTypeId { get; set; }
    }

    public sealed class DocumentTypesDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
