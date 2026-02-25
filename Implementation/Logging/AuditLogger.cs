using Application.Logging;
using DataAccess;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementation.Logging
{
    public sealed class AuditLogger : IAuditLogger
    {
        private readonly DatabaseContext _context;
        public AuditLogger(DatabaseContext context)
        {
            _context = context;
        }

        public async Task LogAsync(AuditLogEntry entry, CancellationToken ct = default)
        {
            _context.AuditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                EventType = entry.EventType,
                ActorId = entry.ActorId,
                ActorEmail = entry.ActorEmail,
                EntityType = entry.EntityType,
                EntityId = entry.EntityId,
                EntityName = entry.EntityName,
                Metadata = entry.Metadata is null
                               ? null
                               : JsonSerializer.Serialize(entry.Metadata),
                IpAddress = entry.IpAddress,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(ct);
        }


    }
}
