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
    public class DocumentTypeFieldOptionConfiguration : EntityConfiguration<DocumentTypeFieldOption, int>
    {
        public override void Configure(EntityTypeBuilder<DocumentTypeFieldOption> builder)
        {
            base.Configure(builder);


            builder.Property(x => x.Value).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Label).HasMaxLength(150).IsRequired();

            builder.HasIndex(x => new { x.FieldDefinitionId, x.Value })
                .IsUnique();

            builder.HasOne(x => x.FieldDefinition)
                .WithMany(fd => fd.Options)
                .HasForeignKey(x => x.FieldDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
