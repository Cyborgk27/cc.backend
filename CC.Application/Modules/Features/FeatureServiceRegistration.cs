using CC.Application.Modules.Features.Interfaces;
using CC.Application.Modules.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CC.Application.Modules.Features
{
    public static class FeatureServiceRegistration
    {
        public static IServiceCollection AddFeatureModule(this IServiceCollection services)
        {
            services.AddScoped<IFeatureApplication, FeatureApplication>();
            return services;
        }
    }
}
