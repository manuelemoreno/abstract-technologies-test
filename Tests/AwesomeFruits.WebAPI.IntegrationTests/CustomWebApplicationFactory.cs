using AwesomeFruits.Application.Mapping.Profiles;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace AwesomeFruits.WebAPI.Integration.Tests;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddAutoMapper(typeof(FruitProfile).Assembly);
            services.AddAutoMapper(typeof(SaveFruitDtoProfile).Assembly);
            services.AddAutoMapper(typeof(UpdateFruitDtoProfile).Assembly);

            // Remove the existing authentication scheme
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(AuthenticationSchemeProvider));
            if (descriptor != null) services.Remove(descriptor);

            // Add authentication using the mock handler
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>("Test", options => { });

            // Make sure to reconfigure the Authentication middleware to use the Test scheme
            services.Configure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            });
        });
    }
}