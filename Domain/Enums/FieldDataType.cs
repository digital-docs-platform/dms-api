using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FieldDataType
    {
        Text = 1,
        Number = 2,
        Date = 3,
        Boolean = 4,
        Decimal = 5,
        Select = 6,
    }


}

