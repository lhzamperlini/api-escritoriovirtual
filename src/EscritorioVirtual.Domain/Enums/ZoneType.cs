using System.Text.Json.Serialization;

namespace EscritorioVirtual.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ZoneType
{
    Desk,
    MeetingRoom,
    Spawn,
    Lounge
}
