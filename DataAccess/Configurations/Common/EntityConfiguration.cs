using DataAccess.Configurations.Common.Extensions;
using Domain.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations.Common
{
    public abstract class EntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
          where TEntity : BaseEntity<TKey>
          where TKey : notnull
    {

        protected virtual bool UseBaseConventions => true;

        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);

            if (typeof(TKey) == typeof(int))
            {
                builder.Property(x => x.Id).ValueGeneratedOnAdd();
            }

            if (typeof(TKey) == typeof(Guid))
            {
                builder.Property(x => x.Id).ValueGeneratedNever();
                // Alternativa: ValueGeneratedOnAdd() (NEWSEQUENTIALID itd.)
            }

            if (UseBaseConventions)
                builder.ApplyBaseConventions();
        }
    }

}
