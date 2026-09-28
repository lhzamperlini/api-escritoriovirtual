namespace EscritorioVirtual.Application.Common.Interfaces.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? Name { get; }
    bool IsAuthenticated { get; }
}
