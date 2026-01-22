using Application.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCaseHandling.CQReslover
{
    public interface ICommandResolver
    {
        public ICommand<TRequest> Resolve<TRequest>();
    }
}
