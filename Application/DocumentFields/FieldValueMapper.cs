using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.DocumentFields
{
    public class FieldValueMapper : IFieldValueMapper
    {
        private delegate bool MapFn(JsonElement value, DocumentTypeFieldValue fv, out string error);

        private static readonly IReadOnlyDictionary<FieldDataType, MapFn> _map =
            new Dictionary<FieldDataType, MapFn>
            {
                [FieldDataType.Text] = MapText,
                [FieldDataType.Number] = MapInt,
                [FieldDataType.Decimal] = MapDecimal,
                [FieldDataType.Boolean] = MapBool,
                [FieldDataType.Date] = MapDate
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

            return fn(value, target, out error);
        }

        private static bool MapText(JsonElement value, DocumentTypeFieldValue fv, out string error)
        {
            error = "";
            fv.ValueString = value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString();
            return true;
        }

        private static bool MapInt(JsonElement value, DocumentTypeFieldValue fv, out string error)
        {
            error = "";
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
            error = "";
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

        private static bool MapBool(JsonElement value, DocumentTypeFieldValue fv, out string error)
        {
            error = "";
            if (value.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                fv.ValueBool = value.GetBoolean();
                return true;
            }

            if (value.ValueKind == JsonValueKind.String &&
                bool.TryParse(value.GetString(), out var b))
            {
                fv.ValueBool = b;
                return true;
            }

            error = "Invalid boolean value.";
            return false;
        }

        private static bool MapDate(JsonElement value, DocumentTypeFieldValue fv, out string error)
        {
            error = "";
            if (value.ValueKind == JsonValueKind.String &&
                DateTime.TryParse(value.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
            {
                fv.ValueDate = dt;
                return true;
            }

            error = "Invalid date value (expected ISO date string).";
            return false;
        }


    }
}
