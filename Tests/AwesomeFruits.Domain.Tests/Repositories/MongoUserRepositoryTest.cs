using System;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Infrastructure.Data.Contexts;
using AwesomeFruits.Infrastructure.Data.Repositories;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace AwesomeFruits.Infrastructure.Unit.Tests.Repositories;

public class MongoUserRepositoryTests : IDisposable
{
    private readonly IMongoDatabase _database;
    private readonly MongoUserRepository _repository;
    private readonly MongoDbRunner _runner;

    public MongoUserRepositoryTests()
    {
        // Start a new MongoDB instance
        _runner = MongoDbRunner.Start();

        // Create a new MongoDbContext using the in-memory MongoDB instance
        var client = new MongoClient(_runner.ConnectionString);
        _database = client.GetDatabase("IntegrationTest");
        var context = new MongoDbContext(_runner.ConnectionString, "IntegrationTest");

        // Initialize the repository to be tested
        _repository = new MongoUserRepository(context);
    }

    public void Dispose()
    {
        // Dispose of the MongoDB instance
        _runner.Dispose();
    }

    [Fact]
    public async void FindByUserNameAsync_UserExists_ReturnsUser()
    {
        // Arrange
        var expectedUser = new User { UserName = "testUser", IsActive = true };
        await _database.GetCollection<User>("Users").InsertOneAsync(expectedUser);

        // Act
        var result = await _repository.FindByUserNameAsync("testUser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.UserName, result.UserName);
    }

    [Fact]
    public async void SaveAsync_NewUser_UserSaved()
    {
        // Arrange
        var newUser = new User { UserName = "newUser", IsActive = false };

        // Act
        var savedUser = await _repository.SaveAsync(newUser);

        // Assert
        var result = await _database.GetCollection<User>("Users").Find(x => x.UserName == "newUser")
            .FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.True(result.IsActive);
    }
}