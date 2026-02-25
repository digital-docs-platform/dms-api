using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Logging
{
    public interface IAuditLogger
    {
        public Task LogAsync(AuditLogEntry entry, CancellationToken ct = default);
    }
}
