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

            builder.Property(x => x.DocumentTypeId)
                 .IsRequired();

            builder.Property(x => x.CreatedBy)
                   .IsRequired();

            builder.Property(d => d.Title)
                 .HasMaxLength(200)
                 .IsRequired();

            builder.HasOne(d => d.CreatedByUser)
                   .WithMany() // ili .WithMany(u => u.CreatedDocuments) ako imaš kolekciju
                   .HasForeignKey(d => d.CreatedBy)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.DocumentVersions)
                .WithOne(v => v.DocumentInstance) // ovo mora da postoji na DocumentVersion
                .HasForeignKey(v => v.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
