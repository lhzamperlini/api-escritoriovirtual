using System.Security.Claims;
using EscritorioVirtual.Domain.AggregateRoot;
using EscritorioVirtual.Infrastructure.Persistence.Contexts;
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
                    var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                    var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;

                    var subClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (subClaim == null) return;

                    var email = claimsIdentity?.FindFirst(ClaimTypes.Email)?.Value ?? "";
                    var fullName = claimsIdentity?.FindFirst("name")?.Value ?? email;

                    if (Guid.TryParse(subClaim, out var userId))
                    {
                        var user = await dbContext.Usuarios.FindAsync(new object[] { userId }, context.HttpContext.RequestAborted);
                        
                        if (user == null)
                        {
                            user = new Usuario(userId, email, fullName);
                            dbContext.Usuarios.Add(user);
                        }
                        else
                        {
                            user.UpdateDetails(email, fullName);
                            user.UpdateLastLogin();
                            dbContext.Usuarios.Update(user);
                        }

                        await dbContext.SaveChangesAsync(context.HttpContext.RequestAborted);
                    }
                }
            };
        });

        return services;
    }
}
