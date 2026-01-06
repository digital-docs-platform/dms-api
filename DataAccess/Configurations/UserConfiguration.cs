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
    public class UserConfiguration : SoftDeletableActivatableConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {

            builder.ToTable("Users");

            base.Configure(builder);

            builder.Property(u => u.FirstName)
                  .IsRequired()
                  .HasMaxLength(100);

            builder.Property(u => u.LastName)
                  .IsRequired()
                  .HasMaxLength(100);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(320);

            builder.Property(u => u.PasswordHash)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(u => u.IsLocked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(u => u.LastLoginUtc)
                .IsRequired(false);

            builder.Property(u => u.JobTitle)
                .IsRequired(false)
                .HasMaxLength(150);
            builder.Property(u => u.Department)
                .IsRequired(false)
                .HasMaxLength(200);



            // Indexes
            builder.HasIndex(u => u.Email)
                   .IsUnique();


        }
    }
}
