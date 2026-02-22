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

            builder.HasOne(ug => ug.User)
                  .WithMany(u => u.UserGroups) // requires User.UserGroups
                  .HasForeignKey(ug => ug.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ug => ug.Group)
                   .WithMany(g => g.GroupUsers)
                   .HasForeignKey(ug => ug.GroupId)
                   .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
