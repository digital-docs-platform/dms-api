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
    public class DocumentTypeFieldDefinitionConfiguration : SoftDeletableConfiguration<DocumentTypeFieldDefinition>
    {
        public override void Configure(EntityTypeBuilder<DocumentTypeFieldDefinition> builder)
        {
           

            builder.ToTable("DocumentTypeFieldDefinitions");

            base.Configure(builder);

            builder.Property(x => x.DocumentTypeId)
                   .IsRequired();

            builder.HasOne(x => x.DocumentType)
                   .WithMany(dt => dt.FieldDefinitions)
                   .HasForeignKey(x => x.DocumentTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Code)
                   .IsRequired()
                   .HasMaxLength(64);

            builder.Property(x => x.Label)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Description)
                   .HasMaxLength(300);

            builder.Property(x => x.DataType)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(30);

            builder.Property(x => x.IsRequired).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.SortOrder).IsRequired();
            builder.Property(x => x.IsSearchable).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.IsSortable).IsRequired().HasDefaultValue(false);

            builder.HasIndex(x => new { x.DocumentTypeId, x.Code })
                   .IsUnique();

        }
    }
}
