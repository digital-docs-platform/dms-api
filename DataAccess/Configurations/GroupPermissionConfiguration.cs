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
    public class GroupPermissionConfiguration : IEntityTypeConfiguration<GroupPermission>
    {
        public void Configure(EntityTypeBuilder<GroupPermission> builder)
        {
            builder.ToTable("GroupPermissions");

            builder.HasKey(gp => new { gp.GroupId, gp.PermissionId });

            builder.Property(gp => gp.AddedAtUtc)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(gp => gp.Group)
            .WithMany(g => g.GroupPermissions)
            .HasForeignKey(gp => gp.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(gp => gp.Permission)
                .WithMany(p => p.PermissionGroups)
                .HasForeignKey(gp => gp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(gp => gp.AddedByUser)
                    .WithMany(u => u.GroupPermissionsAddedByMe)
                    .HasForeignKey(gp => gp.AddedByUserId)
                    .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
