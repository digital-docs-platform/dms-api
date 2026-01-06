using Domain.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations.Common
{
    public abstract class SoftDeletableConfiguration<T> : AuditableConfiguration<T>
        where T : SoftDeletableEntity
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.DeletedAtUtc).IsRequired(false);
        }
    }
}
