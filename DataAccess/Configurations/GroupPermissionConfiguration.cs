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
            builder.HasKey(gp => new { gp.GroupId, gp.PermissionId });

            builder.Property(gp => gp.AddedAtUtc)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne<Group>()
                .WithMany()
                .HasForeignKey(gp => gp.GroupId);

            builder.HasOne<Permission>()
                .WithMany()
                .HasForeignKey(gp => gp.PermissionId);

        }
    }
}
