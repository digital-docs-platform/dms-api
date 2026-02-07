using DataAccess.Configurations.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class GroupConfiguration : EntityConfiguration<Group, Guid>
    {
        public override void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Groups");

            base.Configure(builder);

            builder.Property(g => g.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(g => g.Name).IsUnique();

            builder.Property(g => g.Description).HasMaxLength(300).IsRequired(false);
        }
    }
}
