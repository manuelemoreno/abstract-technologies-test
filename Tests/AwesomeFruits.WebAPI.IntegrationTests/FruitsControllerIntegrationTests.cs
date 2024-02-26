using System.Net;
using System.Text;
using System.Text.Json;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Infrastructure.Data.Contexts;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AwesomeFruits.WebAPI.Integration.Tests;

public class FruitsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Startup> _factory;

    public FruitsControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<SqlDbContext>));

                services.Remove(descriptor);

                services.AddDbContext<SqlDbContext>(options => { options.UseInMemoryDatabase("TestDatabase"); });

                // Initialize the database with test data
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<SqlDbContext>())
                {
                    try
                    {
                        appContext.Database.EnsureCreated();
                    }
                    catch (Exception ex)
                    {
                        var test = "";
                        // Log errors
                    }
                }
            });
            builder.ConfigureKestrel(serverOptions =>
            {
                // Configure Kestrel to use HTTPS
                serverOptions.ListenLocalhost(44334,
                    listenOptions =>
                    {
                        listenOptions.UseHttps("../../AwesomeFruits/cert/awesomefruits.pfx", "12345678");
                    });
            });
            builder.UseEnvironment("Testing");
        });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:44334")
        });

  
    }

    [Fact]
    public async Task Get_ReturnsFruit()
    {
        var fruitName = "TestFruitGet";
        var fruitDesc = "TestDescGet";

        var responsePost = await _client.PostAsync("/Fruits", new StringContent(
            JsonSerializer.Serialize(
                new SaveFruitDto
                {
                    Name = fruitName,
                    Description = fruitDesc
                }),
            Encoding.UTF8,
            "application/json"));

        var responsePostStream = await responsePost.Content.ReadAsStreamAsync();
        var dataResponsePost = await JsonSerializer.DeserializeAsync<Fruit>(responsePostStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var response = await _client.GetAsync($"/Fruits/{dataResponsePost.Id}");

        var responseStream = await response.Content.ReadAsStreamAsync();
        var dataResponse = await JsonSerializer.DeserializeAsync<Fruit>(responseStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        dataResponse?.Name.Should().Be(fruitName);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Save_New_Fruit_Should_Return_Fruit()
    {
        var fruitName = "TestFruitSave";
        var fruitDesc = "TestDescSave";
        var response = await _client.PostAsync("/Fruits", new StringContent(
            JsonSerializer.Serialize(
                new SaveFruitDto
                {
                    Name = fruitName,
                    Description = fruitDesc
                }),
            Encoding.UTF8,
            "application/json"));

        var responseStream = await response.Content.ReadAsStreamAsync();
        var dataResponse = await JsonSerializer.DeserializeAsync<Fruit>(responseStream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        dataResponse?.Name.Should().Be(fruitName);
        dataResponse?.Description.Should().Be(fruitDesc);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Update_Existing_Fruit_Should_Return_NoContent()
    {
        var responsePost = await _client.PostAsync("/Fruits", new StringContent(
            JsonSerializer.Serialize(
                new SaveFruitDto
                {
                    Name = "test",
                    Description = "test"
                }),
            Encoding.UTF8,
            "application/json"));

        var responsePostStream = await responsePost.Content.ReadAsStreamAsync();
        var dataResponsePost = await JsonSerializer.DeserializeAsync<Fruit>(responsePostStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var fruitName = "TestFruitUpdated";
        var fruitDesc = "TestDescUpdated";
        var response = await _client.PutAsync("/Fruits", new StringContent(
            JsonSerializer.Serialize(
                new UpdateFruitDto
                {
                    Id = dataResponsePost.Id,
                    Name = fruitName,
                    Description = fruitDesc
                }),
            Encoding.UTF8,
            "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_Existing_Fruit_Should_Return_NoContent()
    {
        var responsePost = await _client.PostAsync("/Fruits", new StringContent(
            JsonSerializer.Serialize(
                new SaveFruitDto
                {
                    Name = "test",
                    Description = "test"
                }),
            Encoding.UTF8,
            "application/json"));

        var responsePostStream = await responsePost.Content.ReadAsStreamAsync();
        var dataResponsePost = await JsonSerializer.DeserializeAsync<Fruit>(responsePostStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var response = await _client.DeleteAsync($"/Fruits/{dataResponsePost.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}