using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using CleanAdmin.Infrastructure.Utils;

namespace CleanAdmin.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUtilsInfrastructure(this IServiceCollection services)
    {
        // Add infrastructure services here
        services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
        services.AddScoped<IPasswordHasher, IdentityPasswordHasher>();
        return services;
    }
}