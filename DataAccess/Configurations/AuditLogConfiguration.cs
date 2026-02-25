using DataAccess.Configurations.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations
{
    public class AuditLogConfiguration : EntityConfiguration<AuditLog, Guid>
    {
        public override void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.EventType)
                   .IsRequired()
                   .HasConversion<string>();

            builder.Property(x => x.ActorEmail).HasMaxLength(320);
            builder.Property(x => x.EntityType).HasMaxLength(150);
            builder.Property(x => x.IpAddress).HasMaxLength(64);
            builder.Property(x => x.Metadata).HasColumnType("nvarchar(max)");

            // Index za brze query-je
            builder.HasIndex(x => x.ActorId);
            builder.HasIndex(x => new { x.EntityType, x.EntityId });

        }
    }
}
