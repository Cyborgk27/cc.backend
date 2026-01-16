using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CC.Application.Common.Helpers;
using CC.Application.Interfaces;
using CC.Application.Services;

namespace CC.Application.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ServiceData>();

            services.AddScoped<IAuthApplication, AuthApplication>();
            services.AddScoped<IFeatureApplication, FeatureApplication>();
            services.AddScoped<IExternalCatalogApplication, ExternalCatalogApplication>();
            services.AddScoped<IProjectApplication, ProjectApplication>();
            services.AddScoped<ISecurityApplication, SecurityApplication>();
            services.AddScoped<ICatalogApplication, CatalogApplication>();
            services.AddScoped<IUserApplication, UserApplication>();
            services.AddScoped<IDashboardService, DashboardService>();
            return services;
        }
    }
}
