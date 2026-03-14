using CC.Application.Common.Helpers;
using CC.Application.Common.Interfaces;
using CC.Application.Common.Services;
using CC.Application.Modules.Catalogs;
using CC.Application.Modules.Features;
using CC.Application.Modules.Identity;
using CC.Application.Modules.Identity.Interfaces;
using CC.Application.Modules.Identity.Services;
using CC.Application.Modules.Projects;
using CC.Application.Modules.System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CC.Application.Extensions
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddInjectionApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ServiceData>();

            services.AddScoped<IAuthApplication, AuthApplication>();
            services.AddScoped<ICommonApplication, CommonApplication>();

            services.AddIdentityModule();
            services.AddCatalogModule();
            services.AddFeatureModule();
            services.AddProjectModule();
            services.AddSystemModule();
            return services;
        }
    }
}
