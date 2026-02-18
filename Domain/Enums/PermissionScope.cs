



using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PermissionScope
    {
        User = 1,
        DocumentType = 2,
        Document = 3,
        Group = 4,
        System = 5,
        Global = 6
    }
}
