using EscritorioVirtual.Domain.Aggregates.Workspaces;
using EscritorioVirtual.Domain.Enums;

namespace EscritorioVirtual.Domain.AggregateRoot.Workspaces;

public class Workspace : BaseAggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private readonly List<WorkspaceUser> _members = new();
    public virtual IReadOnlyCollection<WorkspaceUser> Members => _members.AsReadOnly();

    private readonly List<WorkspaceInvite> _invites = new();
    public virtual IReadOnlyCollection<WorkspaceInvite> Invites => _invites.AsReadOnly();

    protected Workspace() { }

    public Workspace(string name, string slug, Guid creatorUserId)
    {
        Id = Guid.CreateVersion7();
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        CreatedAt = DateTime.UtcNow;
        DataCriacao = DateTime.UtcNow;
        Status = true;
        UsuarioCriacaoId = creatorUserId;

        // Ao criar, o usuário ganha automaticamente a role de 'Owner'
        _members.Add(new WorkspaceUser(Id, creatorUserId, WorkspaceRole.Owner));
    }

    public void UpdateDetails(string name, string slug)
    {
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        DataAtualizacao = DateTime.UtcNow;
    }

    public void AddMember(Guid userId, WorkspaceRole role)
    {
        var existing = _members.FirstOrDefault(m => m.UserId == userId);
        if (existing != null)
        {
            existing.UpdateRole(role);
        }
        else
        {
            _members.Add(new WorkspaceUser(Id, userId, role));
        }
    }

    public bool RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null) return false;
        return _members.Remove(member);
    }

    public WorkspaceInvite CreateInvite(Guid createdByUserId, WorkspaceRole role, string? email = null, DateTime? expiresAt = null, string? customCode = null)
    {
        var invite = new WorkspaceInvite(Id, createdByUserId, role, email, expiresAt, customCode);
        _invites.Add(invite);
        return invite;
    }
}
