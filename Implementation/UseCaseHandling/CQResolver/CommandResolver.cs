using Application.UseCaseHandling.CQReslover;
using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.UseCaseHandling.CQResolver
{
    public class CommandResolver : ICommandResolver
    {
        private readonly IServiceProvider _sp;
        public CommandResolver(IServiceProvider sp)
        {
            _sp = sp;
        }
        public ICommand<TRequest> Resolve<TRequest>()
        {
            return _sp.GetRequiredService<ICommand<TRequest>>();
        }
    }
}
