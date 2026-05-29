using AccessHub.Application.Interfaces;
using AccessHub.Domain.Entities;
using AccessHub.Infrastructure.Persistence;
using AccessHub.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AccessHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccessHubDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAccessHubDbContext>(sp => sp.GetRequiredService<AccessHubDbContext>());
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AccessHubDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}
