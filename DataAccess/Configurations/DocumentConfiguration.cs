using DataAccess.Configurations.Common;
using Domain.Entities;
using Domain.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Configurations
{
    public class DocumentConfiguration : EntityConfiguration<Document, Guid>
    {
        public override void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            base.Configure(builder);


        }
    }
}
