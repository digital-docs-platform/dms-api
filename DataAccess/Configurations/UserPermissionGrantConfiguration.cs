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
    public class UserPermissionGrantConfiguration : EntityConfiguration<UserPermissionGrant, int>
    {
        public  void Configure(EntityTypeBuilder<UserPermissionGrant> builder)
        {
            base.Configure(builder);

            builder
               .HasIndex(x => new { x.UserId, x.PermissionId, x.DocumentTypeId })
               .IsUnique();

            builder
                .HasOne(x => x.GrantedByUser)
                .WithMany()
                .HasForeignKey(x => x.GrantedByUserId)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
