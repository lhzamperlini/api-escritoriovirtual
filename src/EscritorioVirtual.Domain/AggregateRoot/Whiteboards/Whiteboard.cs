namespace EscritorioVirtual.Domain.AggregateRoot.Whiteboards;

public class Whiteboard : BaseAggregateRoot<Guid>
{
    public Guid WorkspaceId { get; private set; }
    public Guid? ZoneId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string DocumentData { get; private set; } = "{}";
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdated { get; private set; }

    protected Whiteboard() { }

    public Whiteboard(Guid workspaceId, string name, Guid? zoneId = null, string? initialData = null, Guid? id = null)
    {
        Id = id ?? Guid.CreateVersion7();
        WorkspaceId = workspaceId;
        Name = string.IsNullOrWhiteSpace(name) ? "Quadro Branco" : name.Trim();
        ZoneId = zoneId;
        DocumentData = string.IsNullOrWhiteSpace(initialData) ? "{}" : initialData;
        CreatedAt = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
        DataCriacao = DateTime.UtcNow;
        Status = true;
    }

    public void UpdateSnapshot(string documentData)
    {
        DocumentData = string.IsNullOrWhiteSpace(documentData) ? "{}" : documentData;
        LastUpdated = DateTime.UtcNow;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Rename(string newName)
    {
        if (!string.IsNullOrWhiteSpace(newName))
        {
            Name = newName.Trim();
            LastUpdated = DateTime.UtcNow;
            DataAtualizacao = DateTime.UtcNow;
        }
    }
}
