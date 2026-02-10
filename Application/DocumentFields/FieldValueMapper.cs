using Domain.Entities;
using Domain.Enums;
using System.Globalization;
using System.Text.Json;

namespace Application.DocumentFields
{
    public sealed class FieldValueMapper : IFieldValueMapper
    {
        private delegate bool MapFn(JsonElement value, DocumentTypeFieldValue fv, out string error);

        private static readonly IReadOnlyDictionary<FieldDataType, MapFn> _map =
            new Dictionary<FieldDataType, MapFn>
            {
                [FieldDataType.Text] = MapText,
                [FieldDataType.Number] = MapInt,
                [FieldDataType.Decimal] = MapDecimal,
                [FieldDataType.Date] = MapDate,
                [FieldDataType.Select] = MapSelect
            };

        public bool TryApply(FieldDataType type, JsonElement value, DocumentTypeFieldValue target, out string error)
        {
            error = string.Empty;

            if (target is null)
            {
                error = "Field value target is null.";
                return false;
            }

            if (value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
            {
                error = "Value is null.";
                return false;
            }

            if (!_map.TryGetValue(type, out var fn))
            {
                error = "Unsupported data type.";
                return false;
            }

            // IMPORTANT: always clear all columns before setting the new one
            ClearAll(target);

            return fn(value, target, out error);
        }

        private static void ClearAll(DocumentTypeFieldValue fv)
        {
            fv.ValueString = null;
            fv.ValueInt = null;
            fv.ValueDecimal = null;
            fv.ValueDate = null;
            fv.ValueOptionId = null;

        }

        private static bool MapText(JsonElement value, DocumentTypeFieldValue fv, out string error)
        {
            error = string.Empty;

            if (value.ValueKind == JsonValueKind.String)
            {
                fv.ValueString = value.GetString();
                return true;
            }

            // fallback for non-string: store as string representation
            fv.ValueString = value.ToString();
            return true;
        }

        private static bool MapInt(JsonElement value, DocumentTypeFieldValue fv, out string error)
        {
            error = string.Empty;

            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var i))
            {
                fv.ValueInt = i;
                return true;
            }

            if (value.ValueKind == JsonValueKind.String &&
                int.TryParse(value.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out i))
            {
                fv.ValueInt = i;
                return true;
            }

            error = "Invalid integer value.";
            return false;
        }

        private static bool MapDecimal(JsonElement value, DocumentTypeFieldValue fv, out string error)
        {
            error = string.Empty;

            if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var d))
            {
                fv.ValueDecimal = d;
                return true;
            }

            if (value.ValueKind == JsonValueKind.String &&
                decimal.TryParse(value.GetString(), NumberStyles.Number, CultureInfo.InvariantCulture, out d))
            {
                fv.ValueDecimal = d;
                return true;
            }

            error = "Invalid decimal value.";
            return false;
        }

        private static bool MapDate(JsonElement value, DocumentTypeFieldValue fv, out string error)
        {
            error = string.Empty;

            if (value.ValueKind == JsonValueKind.String &&
                DateTime.TryParse(value.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
            {
                fv.ValueDate = dt;
                return true;
            }

            error = "Invalid date value (expected ISO date string).";
            return false;
        }

        // Select: value MUST be optionId (int)
        private static bool MapSelect(JsonElement value, DocumentTypeFieldValue fv, out string error)
        {
            error = string.Empty;

            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var optionId))
            {
                fv.ValueOptionId = optionId;
                return true;
            }

            if (value.ValueKind == JsonValueKind.String &&
                int.TryParse(value.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out optionId))
            {
                fv.ValueOptionId = optionId;
                return true;
            }

            error = "Invalid select value (expected optionId as number).";
            return false;
        }
    }
}
