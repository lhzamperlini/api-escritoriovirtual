using System.Text.Json.Serialization;

namespace EscritorioVirtual.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorkspaceRole
{
    Owner = 1,
    Admin = 2,
    Member = 3,
    Guest = 4
}
