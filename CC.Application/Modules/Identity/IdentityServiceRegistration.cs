using CC.Application.Modules.Identity.Interfaces;
using CC.Application.Modules.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CC.Application.Modules.Identity
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection AddIdentityModule(this IServiceCollection services)
        {
            services.AddScoped<IAuthApplication, AuthApplication>();
            services.AddScoped<ISecurityApplication, SecurityApplication>();
            services.AddScoped<IUserApplication, UserApplication>();
            return services;
        }
    }
}
