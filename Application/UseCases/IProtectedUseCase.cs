using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{


    public interface IProtectedUseCase : IUseCase
    {
        string RequiredPermission { get; } // Documents.Create
        public PermissionScope Scope { get; }
    }
}
