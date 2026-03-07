using DataAccess.Configurations.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public sealed class DocumentVersionFileConfiguration : EntityConfiguration<DocumentVersionFile, Guid>
    {
        public override void Configure(EntityTypeBuilder<DocumentVersionFile> builder)
        {
            base.Configure(builder);

            builder.ToTable("DocumentVersionFiles");

            builder.Property(x => x.DocumentVersionId)
                   .IsRequired();

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(x => x.Extension)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(x => x.ContentType)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.SizeInBytes)
                   .IsRequired();

            builder.Property(x => x.StorageKey)
                   .IsRequired()
                   .HasMaxLength(512);

            builder.Property(x => x.Order)
                   .IsRequired();

            builder.HasIndex(x => x.StorageKey)
                   .IsUnique();
        }
    }
}
