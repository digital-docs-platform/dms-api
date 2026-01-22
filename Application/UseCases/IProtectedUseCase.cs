using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public enum PermissionScope
    {
        Global = 1,
        DocumentType = 2
    }


    public interface IProtectedUseCase : IUseCase
    {
        string RequiredPermission { get; } // Documents.Create
        public PermissionScope Scope { get; }
    }
}
