using Application.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public interface IAuditableUseCase<TInput>
    {
        public AuditLogEntry BuildAuditEntry(TInput input, IApplicationActor actor);
    }
}
