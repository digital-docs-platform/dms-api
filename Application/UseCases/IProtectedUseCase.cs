using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public interface IProtectedUseCase : IUseCase
    {
        IEnumerable<string> RequiredPermissions { get; } // Documents.Create
    }
}
