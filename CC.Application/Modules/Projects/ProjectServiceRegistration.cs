using CC.Application.Modules.Projects.Interfaces;
using CC.Application.Modules.Projects.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CC.Application.Modules.Projects
{
    public static class ProjectServiceRegistration
    {
        public static IServiceCollection AddProjectModule(this IServiceCollection services)
        {
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IProjectApplication, ProjectApplication>();
            return services;
        }
    }
}
