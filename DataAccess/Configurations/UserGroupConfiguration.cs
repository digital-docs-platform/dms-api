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
    public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            builder.ToTable("UserGroups");

            builder.HasKey(x => new { x.UserId, x.GroupId });

            builder.Property(x => x.AddedAtUtc).IsRequired();
            builder.Property(x => x.AddedByUserId).IsRequired(false);

            builder.HasOne<User>()
                .WithMany(u => u.UserGroups)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Group>()
                .WithMany(g => g.GroupUsers)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
