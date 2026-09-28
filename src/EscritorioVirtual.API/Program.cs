using dotenv.net;
using EscritorioVirtual.API.Middlewares;
using EscritorioVirtual.Application.DependencyInjection;
using EscritorioVirtual.Infrastructure.Configurations;
using EscritorioVirtual.Infrastructure.Extensions;
using EscritorioVirtual.Infrastructure.Persistence;
using EscritorioVirtual.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var frontendUrl = builder.Configuration["FRONTEND_URL"] ?? "http://localhost:4200";
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration["POSTGRES_CONNECTION_STRING"]);
});

builder.Services.AddOidcAuthentication(builder.Configuration);

builder.Services.ConfigureMediatr();
builder.Services.AddInfrastructureServices();

var app = builder.Build();

// Garante a criação automática das tabelas mapeadas sem depender de DbSet no AppDbContext nem migrations manuais
await app.Services.EnsureDatabaseAndTablesCreatedAsync();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<WorkspaceTenantMiddleware>();

try
{
    app.MapControllers();
    app.Run();
}
catch (System.Reflection.ReflectionTypeLoadException ex)
{
    foreach (var loaderException in ex.LoaderExceptions)
    {
        Console.WriteLine($"LoaderException: {loaderException?.Message}");
    }
    throw;
}
