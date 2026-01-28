using Domain.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations.Common.Extensions
{
    public static class EntityTypeBuilderConventions
    {
        public static void ApplyBaseConventions<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class
        {
            var t = typeof(TEntity);

            if (typeof(IAuditable).IsAssignableFrom(t))
            {
                builder.Property<DateTime>(nameof(IAuditable.CreatedAt)).IsRequired();
                builder.Property<DateTime?>(nameof(IAuditable.ModifiedAt));
            }

            if (typeof(IActivatable).IsAssignableFrom(t))
            {
                builder.Property<bool>(nameof(IActivatable.IsActive)).IsRequired();
                builder.HasIndex(nameof(IActivatable.IsActive));
            }

            if (typeof(ISoftDeletable).IsAssignableFrom(t))
            {
                builder.Property<bool>(nameof(ISoftDeletable.IsDeleted)).IsRequired();
                builder.Property<DateTime?>(nameof(ISoftDeletable.DeletedAt));

                builder.HasIndex(nameof(ISoftDeletable.IsDeleted));
            }
        }

    }
}
