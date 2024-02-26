using AwesomeFruits.Application.Interfaces;
using AwesomeFruits.Application.Services;
using AwesomeFruits.Domain.Interfaces;
using AwesomeFruits.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AwesomeFruits.WebAPI.Extensions;

/// <summary>
///     The ServiceRegistrationExtension
/// </summary>
public static class ServiceRegistrationExtension
{

    /// <summary>Adds services to the IServiceCollection</summary>
    /// <param name="services">The services.</param>
    /// <returns>
    ///     IServiceCollection
    /// </returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IFruitService, FruitService>();
        //  services.AddScoped<IFruitRepository, MongoFruitRepository>();
        services.AddScoped<IFruitRepository, SqlFruitRepository>();

        return services;
    }
}