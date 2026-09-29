using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace EscritorioVirtual.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    private string GetRedirectUrl(string? returnUrl)
    {
        var frontendUrl = configuration["FRONTEND_URL"]?.TrimEnd('/') ?? "http://localhost:4200";

        if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl == "/")
        {
            return $"{frontendUrl}/dashboard";
        }

        if (Uri.TryCreate(returnUrl, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.ToString();
        }

        return $"{frontendUrl}/{returnUrl.TrimStart('/')}";
    }

    [HttpGet("login")]
    public IActionResult Login([FromQuery] string? returnUrl = null)
    {
        var targetUrl = GetRedirectUrl(returnUrl);
        return Challenge(new AuthenticationProperties { RedirectUri = targetUrl }, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet("logout")]
    public IActionResult Logout([FromQuery] string? returnUrl = null)
    {
        var frontendUrl = configuration["FRONTEND_URL"]?.TrimEnd('/') ?? "http://localhost:4200";
        var targetUrl = string.IsNullOrWhiteSpace(returnUrl) ? $"{frontendUrl}/login" : GetRedirectUrl(returnUrl);
        return SignOut(new AuthenticationProperties { RedirectUri = targetUrl }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var claims = User.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.Count() > 1 ? (object)g.Select(x => x.Value).ToArray() : g.First().Value);

            var name = User.FindFirst("name")?.Value 
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value 
                    ?? User.Identity.Name 
                    ?? "Usuário";
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                     ?? User.FindFirst("email")?.Value 
                     ?? "";

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? User.FindFirst("sub")?.Value 
                      ?? "";

            return Ok(new
            {
                id = userId,
                isAuthenticated = true,
                name,
                email,
                claims
            });
        }

        return Unauthorized();
    }
}
