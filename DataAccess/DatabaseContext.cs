using Domain.Entities;
using Domain.Entities.BaseEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<UserPermissionGrant> UserPermissionGrants { get; set; }
        public DbSet<DocumentTypeFieldDefinition> DocumentTypeFieldDefinitions { get; set; }
        public DbSet<DocumentTypeFieldValue> DocumentTypeFieldValues { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        public DbSet<DocumentTypeFieldOption> DocumentTypeFieldOptions {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        }

        #region SaveChangesOverride
        public override int SaveChanges()
        {
            ApplyAuditRules();
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditRules();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditRules()
        {
            var now = DateTime.UtcNow;

            // Auditable: Created/Modified
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAt = now;
                    entry.Property(e => e.CreatedAt).IsModified = false;
                }
            }

            // Soft delete: samo za SoftDeletableEntity
            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.IsDeleted = true;
                }
            }


        }


        #endregion


    }
}
