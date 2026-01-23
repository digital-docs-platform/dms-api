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
    public sealed class DocumentVersionConfiguration : SoftDeletableConfiguration<DocumentVersion>
    {
        public override void Configure(EntityTypeBuilder<DocumentVersion> builder)
        {
            base.Configure(builder);

            builder.ToTable("DocumentVersions");

            builder.Property(x => x.DocumentId)
                   .IsRequired();

            builder.Property(x => x.VersionNumber)
                   .IsRequired();

            builder.Property(x => x.FileName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.ContentType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.FileSizeBytes)
                   .IsRequired();

            builder.Property(x => x.StorageKey)
                   .IsRequired()
                   .HasMaxLength(512);

         
            builder.HasIndex(x => new { x.DocumentId, x.VersionNumber })
                   .IsUnique();

          
            builder.HasOne<Document>()               
                   .WithMany()                       
                   .HasForeignKey(x => x.DocumentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.FieldValues)
                   .WithOne(x => x.Version)
                   .HasForeignKey(x => x.VersionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
