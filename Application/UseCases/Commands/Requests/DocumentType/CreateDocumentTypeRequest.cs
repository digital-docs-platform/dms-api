using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Commands.Requests.DocumentType
{
    public sealed class CreateDocumentTypeRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IReadOnlyCollection<CreateDocumentTypeFieldRequest> Fields { get; set; } = new List<CreateDocumentTypeFieldRequest>();
    }

    public sealed class CreateDocumentTypeFieldRequest
    {
        public string Label { get; set; }
        public string Description { get; set; }


        public FieldDataType DataType { get; set; }

        public int SortOrder { get; set; }
        public bool IsRequired { get; set; }
        public bool IsSearchable { get; set; }
        public bool IsSortable { get; set; }
    }
}
