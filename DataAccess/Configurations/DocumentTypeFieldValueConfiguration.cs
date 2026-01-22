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
    public class DocumentTypeFieldValueConfiguration : AuditableConfiguration<DocumentTypeFieldValue>
    {
        public override void Configure(EntityTypeBuilder<DocumentTypeFieldValue> builder)
        {
            base.Configure(builder);

            builder.ToTable("DocumentTypeFieldValues");

            builder.Property(x => x.VersionId).IsRequired();
            builder.Property(x => x.FieldDefinitionId).IsRequired();

            builder.Property(x => x.ValueString)
                   .HasMaxLength(2000); 

            builder.Property(x => x.ValueDecimal)
                   .HasPrecision(18, 4);

         
            builder.HasIndex(x => new { x.VersionId, x.FieldDefinitionId })
                   .IsUnique();

            // Indeksi za pretragu (opciono ali korisno)
            //builder.HasIndex(x => new { x.FieldDefinitionId, x.ValueString });
            //builder.HasIndex(x => new { x.FieldDefinitionId, x.ValueInt });
            //builder.HasIndex(x => new { x.FieldDefinitionId, x.ValueDecimal });
            //builder.HasIndex(x => new { x.FieldDefinitionId, x.ValueDate });
            //builder.HasIndex(x => new { x.FieldDefinitionId, x.ValueBool });

          
            builder.HasOne(x => x.FieldDefinition)
                   .WithMany() 
                   .HasForeignKey(x => x.FieldDefinitionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Version)
                   .WithMany()
                   .HasForeignKey(x => x.VersionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // CHECK constraint: mora biti popunjeno TAČNO JEDNO od Value* polja
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_DocumentTypeFieldValues_ExactlyOneValue",
                @"(
                    (CASE WHEN [ValueString]  IS NULL THEN 0 ELSE 1 END) +
                    (CASE WHEN [ValueInt]     IS NULL THEN 0 ELSE 1 END) +
                    (CASE WHEN [ValueDecimal] IS NULL THEN 0 ELSE 1 END) +
                    (CASE WHEN [ValueDate]    IS NULL THEN 0 ELSE 1 END) +
                    (CASE WHEN [ValueBool]    IS NULL THEN 0 ELSE 1 END)
                  ) = 1"
            ));
        }
    }
}
