namespace EscritorioVirtual.Domain.Aggregates.Presence;

public class UserPresence
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarConfig { get; set; }
    public Guid WorkspaceId { get; set; }
    public Guid MapId { get; set; }
    public string ConnectionId { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public int GridX { get; set; }
    public int GridY { get; set; }
    public string Direction { get; set; } = "down";
    public bool IsMoving { get; set; }
    public string Status { get; set; } = "available"; // available, focus, busy, away, reconnecting
    public Guid? CurrentZoneId { get; set; }
    public long LastHeartbeat { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    public bool IsGracePeriodExpired(long currentTimestamp, int timeoutSeconds = 15)
    {
        return (currentTimestamp - LastHeartbeat) > timeoutSeconds;
    }

    public void TouchHeartbeat()
    {
        LastHeartbeat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (Status == "reconnecting")
        {
            Status = "available";
        }
    }
}
