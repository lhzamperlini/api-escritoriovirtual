using EscritorioVirtual.Domain.AggregateRoot;
using EscritorioVirtual.Domain.AggregateRoot.Workspaces;
using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Domain.Aggregates.Workspaces;

public class WorkspaceInvite
{
    public Guid Id { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public Workspace Workspace { get; private set; } = null!;

    public string Code { get; private set; } = null!;
    public string? Email { get; private set; }
    public WorkspaceRole RoleId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Usuario CreatedByUser { get; private set; } = null!;
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; }

    protected WorkspaceInvite() { }

    public WorkspaceInvite(Guid workspaceId, Guid createdByUserId, WorkspaceRole role, string? email = null, DateTime? expiresAt = null, string? customCode = null)
    {
        Id = Guid.CreateVersion7();
        WorkspaceId = workspaceId;
        CreatedByUserId = createdByUserId;
        RoleId = role;
        Email = email?.Trim().ToLowerInvariant();
        ExpiresAt = expiresAt;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        Code = customCode ?? GenerateInviteCode();
    }

    public bool IsValid() => IsActive && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);

    public void Deactivate() => IsActive = false;

    private static string GenerateInviteCode()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .Replace("=", "")
            .Substring(0, 10);
    }
}
