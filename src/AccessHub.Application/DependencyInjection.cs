using AccessHub.Application.Interfaces;
using AccessHub.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AccessHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPermissionService, PermissionService>();
        return services;
    }
}
