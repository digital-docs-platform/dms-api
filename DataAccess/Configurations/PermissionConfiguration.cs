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
    public class PermissionConfiguration : AuditableConfiguration<Permission>
    {
        public override void Configure(EntityTypeBuilder<Permission> builder)
        {


            builder.ToTable("Permissions");

            base.Configure(builder);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            



            //Indexes
            builder.HasIndex(p => p.Name).IsUnique();
            builder.HasIndex(p => p.Code).IsUnique();


            DateTime seedTime = new DateTime(2026, 01, 10, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(

              

              // Dokumenti - osnovno
              new Permission { Id = 1, Code = "documents.read", Name = "Read documents", Description = "View/list documents", CreatedAtUtc = seedTime },
              new Permission { Id = 2, Code = "documents.write", Name = "Create documents", Description = "Create new document", CreatedAtUtc = seedTime },
              new Permission { Id = 4, Code = "documents.delete", Name = "Delete documents", Description = "Delete/soft-delete document", CreatedAtUtc = seedTime },

              // Verzije / fajlovi
              new Permission { Id = 5, Code = "documents.version.add", Name = "Add version", Description = "Add new document version", CreatedAtUtc = seedTime },
              new Permission { Id = 6, Code = "documents.download", Name = "Download file", Description = "Download document file", CreatedAtUtc = seedTime },
              new Permission { Id = 7, Code = "documents.upload", Name = "Upload file", Description = "Upload document file", CreatedAtUtc = seedTime },

              // Metapodaci (EAV)
              new Permission { Id = 8, Code = "documents.metadata.edit", Name = "Edit metadata", Description = "Edit document metadata fields", CreatedAtUtc = seedTime },

              // Tipovi dokumenata i šeme (admin)
              new Permission { Id = 9, Code = "documentTypes.read", Name = "Read document types", Description = "View document types", CreatedAtUtc = seedTime },
              new Permission { Id = 10, Code = "documentTypes.manage", Name = "Manage document types", Description = "Create/update document types and fields", CreatedAtUtc = seedTime },

              // Korisnici / permisije (admin)
              new Permission { Id = 11, Code = "users.read", Name = "Read users", Description = "View users", CreatedAtUtc = seedTime },
              new Permission { Id = 12, Code = "users.manage", Name = "Manage users", Description = "Create/update/block users", CreatedAtUtc = seedTime },
              new Permission { Id = 13, Code = "permissions.manage", Name = "Manage permissions", Description = "Grant/revoke permissions", CreatedAtUtc = seedTime },

              // Sistem / platforma (globalno)
              new Permission { Id = 14, Code = "system.admin", Name = "System admin", Description = "Administrative access within an organization (global)", CreatedAtUtc = seedTime }
            


            );

        }
    }
}
