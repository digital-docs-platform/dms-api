using Application.UseCases.Commands.Requests.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Commands
{
    public interface IUpdateUserPermissionsCommand : ICommand<UpdateUserPermissionsRequest>, IProtectedUseCase
    {
    }
}
