using Application.UseCases.Commands.Requests.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Commands
{
    public interface IAddUserToGroupCommand : ICommand<AddUserToGroupRequest>, IProtectedUseCase
    {
    }
}
