using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Implementation.Querying.DocumentTypeFieldQuerying
{
    public static class FilterJson
    {
        // Reads "op" from { op: "...", ... }
        public static string? GetOp(JsonElement filter)
        {
            if (filter.ValueKind != JsonValueKind.Object) return null;

            return filter.TryGetProperty("op", out var opEl) && opEl.ValueKind == JsonValueKind.String
                ? opEl.GetString()
                : null;
        }

        // Reads string from { value: "..." }
        public static string? GetStringValue(JsonElement filter)
        {
            if (filter.ValueKind != JsonValueKind.Object) return null;
            if (!filter.TryGetProperty("value", out var el)) return null;
            return el.ValueKind == JsonValueKind.String ? el.GetString() : null;
        }

        // Reads bool from { value: true/false }
        public static bool TryGetBoolValue(JsonElement filter, out bool value)
        {
            value = default;
            if (filter.ValueKind != JsonValueKind.Object) return false;
            if (!filter.TryGetProperty("value", out var el)) return false;

            if (el.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                value = el.GetBoolean();
                return true;
            }

            return false;
        }

        // Reads int from { value: 123 }
        public static bool TryGetIntValue(JsonElement filter, out int value)
        {
            value = default;
            if (filter.ValueKind != JsonValueKind.Object) return false;
            if (!filter.TryGetProperty("value", out var el)) return false;

            return el.ValueKind == JsonValueKind.Number && el.TryGetInt32(out value);
        }

        // Reads decimal from { value: 123.45 }
        public static bool TryGetDecimalValue(JsonElement filter, out decimal value)
        {
            value = default;
            if (filter.ValueKind != JsonValueKind.Object) return false;
            if (!filter.TryGetProperty("value", out var el)) return false;

            return el.ValueKind == JsonValueKind.Number && el.TryGetDecimal(out value);
        }

        // Reads DateTime from { value: "2024-06-11T16:30:00Z" }
        public static bool TryGetDateTimeValue(JsonElement filter, out DateTime value)
        {
            value = default;
            if (filter.ValueKind != JsonValueKind.Object) return false;
            if (!filter.TryGetProperty("value", out var el)) return false;
            if (el.ValueKind != JsonValueKind.String) return false;

            var s = el.GetString();
            if (string.IsNullOrWhiteSpace(s)) return false;

            return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out value);
        }

        // Reads decimal from { min: 1, max: 10 } etc.
        public static decimal? GetDecimal(JsonElement filter, string prop)
        {
            if (filter.ValueKind != JsonValueKind.Object) return null;
            if (!filter.TryGetProperty(prop, out var el)) return null;

            return el.ValueKind == JsonValueKind.Number && el.TryGetDecimal(out var d)
                ? d
                : null;
        }

        // Reads int from { min: 1, max: 10 } etc.
        public static int? GetInt(JsonElement filter, string prop)
        {
            if (filter.ValueKind != JsonValueKind.Object) return null;
            if (!filter.TryGetProperty(prop, out var el)) return null;

            return el.ValueKind == JsonValueKind.Number && el.TryGetInt32(out var i)
                ? i
                : null;
        }

        // Reads DateTime from { from: "...", to: "..." }
        public static DateTime? GetDateTime(JsonElement filter, string prop)
        {
            if (filter.ValueKind != JsonValueKind.Object) return null;
            if (!filter.TryGetProperty(prop, out var el)) return null;
            if (el.ValueKind != JsonValueKind.String) return null;

            var s = el.GetString();
            if (string.IsNullOrWhiteSpace(s)) return null;

            return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt)
                ? dt
                : null;
        }

        // For MultiSelect { values: ["a","b"] }
        public static IReadOnlyList<string> GetStringArray(JsonElement filter, string prop = "values")
        {
            if (filter.ValueKind != JsonValueKind.Object) return Array.Empty<string>();
            if (!filter.TryGetProperty(prop, out var el)) return Array.Empty<string>();
            if (el.ValueKind != JsonValueKind.Array) return Array.Empty<string>();

            var list = new List<string>();
            foreach (var item in el.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.String)
                {
                    var s = item.GetString();
                    if (!string.IsNullOrWhiteSpace(s))
                        list.Add(s.Trim());
                }
            }

            return list;
        }

        public static IReadOnlyList<int> GetIntArray(JsonElement filter, string prop = "values")
        {
            if (filter.ValueKind != JsonValueKind.Object) return Array.Empty<int>();
            if (!filter.TryGetProperty(prop, out var el)) return Array.Empty<int>();
            if (el.ValueKind != JsonValueKind.Array) return Array.Empty<int>();

            var list = new List<int>();

            foreach (var item in el.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out var n))
                    list.Add(n);
            }

            return list;
        }
    }
}
