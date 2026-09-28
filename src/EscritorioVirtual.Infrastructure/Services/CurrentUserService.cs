using System.Security.Claims;
using EscritorioVirtual.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace EscritorioVirtual.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var subClaim = _user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(subClaim, out var id) ? id : null;
        }
    }

    public string? Email =>
        _user?.FindFirst(ClaimTypes.Email)?.Value ??
        _user?.FindFirst("email")?.Value;

    public string? Name =>
        _user?.FindFirst("name")?.Value ??
        _user?.FindFirst(ClaimTypes.Name)?.Value ??
        _user?.Identity?.Name;

    public bool IsAuthenticated => _user?.Identity?.IsAuthenticated == true;
}
