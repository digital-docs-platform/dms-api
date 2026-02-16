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
    public class PermissionDependencyConfiguration : IEntityTypeConfiguration<PermissionDependency>
    {
        public void Configure(EntityTypeBuilder<PermissionDependency> builder)
        {
            builder.ToTable("PermissionDependencies");

            builder.HasKey(x => new { x.PermissionId, x.DependsOnPermissionId });


            builder.Property(x => x.CreatedAt).IsRequired();

            // Self-referencing relationships
            builder.HasOne(x => x.Permission)
                   .WithMany(p => p.Dependencies)
                   .HasForeignKey(x => x.PermissionId)
                   .OnDelete(DeleteBehavior.Cascade); // kad obrišeš permission, briše se i dependency red

            builder.HasOne(x => x.DependsOnPermission)
                   .WithMany(p => p.DependedOnBy)
                   .HasForeignKey(x => x.DependsOnPermissionId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Index za brže lookup-e
            builder.HasIndex(x => x.PermissionId);
            builder.HasIndex(x => x.DependsOnPermissionId);

            // permission ne sme da zavisi sam od sebe
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_PermissionDependencies_NoSelfDependency",
                "[PermissionId] <> [DependsOnPermissionId]"
            ));
        }
    }
}
