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
    public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> builder)
        {

            builder.HasKey(up => new {up.UserId, up.PermissionId});

            builder.Property(up => up.AddedAtUtc)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(up => up.UserId);

            builder.HasOne<Permission>()
                .WithMany()
                .HasForeignKey(up => up.PermissionId);



        }
    }
}
