using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CC.Application.Common.Helpers;
using CC.Application.Modules.Identity.Interfaces;
using CC.Application.Modules.Catalogs.Interfaces;
using CC.Application.Modules.Projects.Interfaces;
using CC.Application.Modules.Features.Interfaces;
using CC.Application.Common.Interfaces;
using CC.Application.Modules.Catalogs.Services;
using CC.Application.Modules.Features.Services;
using CC.Application.Modules.Identity.Services;
using CC.Application.Modules.Projects.Services;
using CC.Application.Common.Services;

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
            services.AddScoped<ICommonApplication, CommonApplication>();
            return services;
        }
    }
}
