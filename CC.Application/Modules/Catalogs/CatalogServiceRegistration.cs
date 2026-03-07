using CC.Application.Modules.Catalogs.Interfaces;
using CC.Application.Modules.Catalogs.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CC.Application.Modules.Catalogs
{
    public static class CatalogServiceRegistration
    {
        public static IServiceCollection AddCatalogModule(this IServiceCollection services)
        {
            services.AddScoped<ICatalogApplication, CatalogApplication>();
            services.AddScoped<IExternalCatalogApplication, ExternalCatalogApplication>();
            return services;
        }
    }
}
