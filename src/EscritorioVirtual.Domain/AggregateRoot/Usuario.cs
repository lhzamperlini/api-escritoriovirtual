namespace EscritorioVirtual.Domain.AggregateRoot;

public class Usuario : BaseAggregateRoot<Guid>
{
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    protected Usuario() { }

    public Usuario(Guid id, string email, string fullName)
    {
        Id = id;
        Email = email;
        FullName = fullName;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string email, string fullName)
    {
        Email = email;
        FullName = fullName;
    }
}
