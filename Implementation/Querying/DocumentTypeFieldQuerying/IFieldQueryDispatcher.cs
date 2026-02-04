using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementation.Querying.DocumentTypeFieldQuerying
{
    public interface IFieldQueryDispatcher
    {
        IQueryable<DocumentVersion> ApplyFilter(
           IQueryable<DocumentVersion> query,
           DocumentTypeFieldDefinition definition,
           JsonElement filter);

        IQueryable<DocumentVersion> ApplySort(
            IQueryable<DocumentVersion> query,
            DocumentTypeFieldDefinition definition,
            bool desc);
    }
}
