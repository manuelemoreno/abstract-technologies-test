using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AwesomeFruits.WebAPI.Extensions;

/// <summary>
///     The SwaggerRegistrationExtensions
/// </summary>
public static class SwaggerRegistrationExtensions
{
    /// <summary>Adds the swagger configurations.</summary>
    /// <param name="services">The services.</param>
    public static IServiceCollection AddSwaggerConfigurations(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "AwesomeFruits.WebAPI", Version = "v1" });
        });
        services.AddSwaggerGen(c =>
        {
            // Define the Bearer Authentication scheme
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Add: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }
}