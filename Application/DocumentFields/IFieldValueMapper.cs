using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.DocumentFields
{
    public interface IFieldValueMapper
    {
        bool TryApply(FieldDataType type, JsonElement value, DocumentTypeFieldValue target, out string error);
    }
}
