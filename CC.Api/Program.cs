using CC.Api.Middleware;
using CC.Application.Extensions;
using CC.Domain.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var enableAuth = configuration.GetValue<bool>("SecuritySettings:EnableJwtAuthentication");

// =====================
// Inyección de capas
// =====================
builder.Services.AddInjectionInfrastructure(configuration);
builder.Services.AddInjectionApplication(configuration);

// =====================
// Controllers
// =====================
builder.Services.AddControllers();

// =====================
// Swagger
// =====================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CC API",
        Version = "v1"
    });

    if (enableAuth)
    {
        var securitySchemeId = "Bearer";

        // 1. Definición del esquema
        c.AddSecurityDefinition(securitySchemeId, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Introduce el token: Bearer {tu_token}",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference(securitySchemeId),
                new List<string>()
            }
        });
    }
});


// =====================
// Build
// =====================
var app = builder.Build();

// =====================
// Middlewares
// =====================
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CC API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

if (enableAuth)
{
    app.UseAuthentication();
    app.UseMiddleware<AuthMiddleware>();
    app.UseAuthorization();
}

app.MapControllers();
app.Run();
