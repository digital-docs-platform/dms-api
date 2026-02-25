using Application.UseCases.Commands.Requests.User;
using Application.UseCases.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Commands
{
    public interface ICreateUserCommand 
                    : ICommand<CreateUserRequest>,
                    IProtectedUseCase,
                    IAuditableUseCase<CreateUserRequest>
    {

    }
}
