using System.Security.Claims;
using EscritorioVirtual.Application.Administracao.Usuarios.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EscritorioVirtual.Infrastructure.Configurations;

public static class ConfigureAuthentication
{
    public static IServiceCollection AddOidcAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.Cookie.Name = "EscritorioVirtual.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = configuration["ASPNETCORE_ENVIRONMENT"] == "Development"
                ? Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest
                : Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
        })
        .AddOpenIdConnect(options =>
        {
            options.Authority = configuration["OIDC_AUTHORITY"];
            options.ClientId = configuration["OIDC_CLIENT_ID"];
            options.ClientSecret = configuration["OIDC_CLIENT_SECRET"];
            options.ResponseType = "code";
            options.SaveTokens = true;
            options.RequireHttpsMetadata = configuration["OIDC_REQUIRE_HTTPS_METADATA"] == "true";
            options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

            options.Events = new OpenIdConnectEvents
            {
                OnTokenValidated = async context =>
                {
                    var usuarioRepository = context.HttpContext.RequestServices.GetRequiredService<IUsuarioRepository>();
                    var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;

                    var subClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (subClaim == null || !Guid.TryParse(subClaim, out var userId)) return;

                    var email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value ?? "";
                    var fullName = claimsIdentity?.FindFirst("name")?.Value ?? email;

                    await usuarioRepository.SincronizarUsuarioAsync(userId, email, fullName, context.HttpContext.RequestAborted);
                }
            };
        });

        return services;
    }
}
