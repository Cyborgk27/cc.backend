using CC.Application.Modules.System.Interfaces;
using CC.Application.Modules.System.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CC.Application.Modules.System
{
    public static class SystemServiceRegistration
    {
        public static IServiceCollection AddSystemModule(this IServiceCollection services)
        {
            services.AddScoped<IAuditApplication, AuditApplication>();
            return services;
        }
    }
}
