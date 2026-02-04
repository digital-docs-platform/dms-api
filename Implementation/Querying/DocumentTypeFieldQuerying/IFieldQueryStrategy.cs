using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementation.Querying.DocumentTypeFieldQuerying
{
    public interface IFieldQueryStrategy
    {
        FieldDataType DataType { get; }

        // Neki tipovi (npr MultiSelect) nemaju smislen sort
        bool CanSort { get; }

        IQueryable<DocumentVersion> ApplyFilter(
            IQueryable<DocumentVersion> query,
            int fieldDefinitionId,
            JsonElement filter);

        IQueryable<DocumentVersion> ApplySort(
            IQueryable<DocumentVersion> query,
            int fieldDefinitionId,
            bool desc);
    }
}
