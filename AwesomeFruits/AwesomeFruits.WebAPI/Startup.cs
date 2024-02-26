using System;
using System.Text;
using AwesomeFruits.Application.Mapping.Profiles;
using AwesomeFruits.Infrastructure.Data.Contexts;
using AwesomeFruits.WebAPI.Extensions;
using AwesomeFruits.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace AwesomeFruits.WebAPI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddServices();

        services.AddDbContext<SqlDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddAutoMapper(typeof(FruitProfile).Assembly);
        services.AddAutoMapper(typeof(SaveFruitDtoProfile).Assembly);
        services.AddAutoMapper(typeof(UpdateFruitDtoProfile).Assembly);

        var jwtSettings = Configuration.GetSection("Jwt");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["IssuerKey"])),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["IssuerName"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddSwaggerConfigurations();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsEnvironment("Testing"))
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<SqlDbContext>();
                dbContext.Database.Migrate(); // Apply migrations
            }
        }
       

        app.UseMiddleware<RequestResponseLoggingMiddleware>();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AwesomeFruits.WebAPI v1"));
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}