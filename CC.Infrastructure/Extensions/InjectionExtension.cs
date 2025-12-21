using CC.Application.Interfaces;
using CC.Domain.Repositories;
using CC.Infrastructure.Persistences.Contexts;
using CC.Infrastructure.Persistences.Repositories;
using CC.Infrastructure.Security;
using CC.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CC.Domain.Extensions
{
    public static class InjectionExtension
    {
        public static IServiceCollection AddInjectionInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = typeof(AppDbContext).Assembly.FullName;
            var connection = configuration.GetConnectionString("SQLiteConnection");

            services.AddDbContext<AppDbContext>(
                options => options.UseSqlite(connection,
                b => b.MigrationsAssembly(assembly)),
                ServiceLifetime.Transient
                );

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IEmailService, EmailService>();

            var enableAuth = configuration.GetValue<bool>("SecuritySettings:EnableJwtAuthentication");

            if (enableAuth)
            {
                // Cambiamos la configuración para definir explícitamente los esquemas por defecto
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                    };
                });

                // Caso TRUE: La autorización requiere un usuario autenticado
                services.AddAuthorization(options =>
                {
                    options.FallbackPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();
                });

            }
            else
            {
                // Caso FALSE: Creamos una política que permite a usuarios anónimos
                // Esto hace que el atributo [Authorize] sea ignorado silenciosamente
                services.AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAssertion(_ => true)
                        .Build();

                    options.FallbackPolicy = options.DefaultPolicy;
                });
            }

            services.AddAuthorization();

            return services;
        }
    }
}
