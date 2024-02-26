using System;
using System.Linq;
using System.Threading.Tasks;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Infrastructure.Data.Contexts;
using AwesomeFruits.Infrastructure.Data.Repositories;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace AwesomeFruits.Infrastructure.Unit.Tests.Repositories;

public class MongoFruitRepositoryTests : IDisposable
{
    private readonly IMongoDatabase _database;
    private readonly MongoFruitRepository _repository;
    private readonly MongoDbRunner _runner;

    public MongoFruitRepositoryTests()
    {
        // Start a new MongoDB instance
        _runner = MongoDbRunner.Start();

        // Create a new MongoDbContext using the in-memory MongoDB instance
        var client = new MongoClient(_runner.ConnectionString);
        _database = client.GetDatabase("IntegrationTest");
        var context = new MongoDbContext(_runner.ConnectionString, "IntegrationTest");

        // Initialize the repository to be tested
        _repository = new MongoFruitRepository(context);
    }

    public void Dispose()
    {
        // Dispose of the MongoDB instance
        _runner.Dispose();
    }

    [Fact]
    public async Task FindAllAsync_ReturnsAllFruits()
    {
        // Arrange
        await _database.GetCollection<Fruit>("Fruits").InsertManyAsync(new[]
        {
            new Fruit { Name = "Apple", Id = Guid.NewGuid() },
            new Fruit { Name = "Banana", Id = Guid.NewGuid() }
        });

        // Act
        var results = await _repository.FindAllAsync();

        // Assert
        Assert.Equal(2, results.Count());
    }

    [Fact]
    public async Task FindByNameAsync_FruitExists_ReturnsFruit()
    {
        // Arrange
        var fruit = new Fruit { Name = "Mango", Id = Guid.NewGuid() };
        await _database.GetCollection<Fruit>("Fruits").InsertOneAsync(fruit);

        // Act
        var result = await _repository.FindByNameAsync("Mango");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Mango", result.Name);
    }

    [Fact]
    public async Task FindByIdAsync_FruitExists_ReturnsFruit()
    {
        // Arrange
        var fruitId = Guid.NewGuid();
        var fruit = new Fruit { Name = "Peach", Id = fruitId };
        await _database.GetCollection<Fruit>("Fruits").InsertOneAsync(fruit);

        // Act
        var result = await _repository.FindByIdAsync(fruitId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fruitId, result.Id);
    }

    [Fact]
    public async Task SaveAsync_SavesFruit_ReturnsFruit()
    {
        // Arrange
        var fruit = new Fruit { Name = "Cherry", Id = Guid.NewGuid() };

        // Act
        var savedFruit = await _repository.SaveAsync(fruit);

        // Assert
        var result = await _database.GetCollection<Fruit>("Fruits").Find(x => x.Id == savedFruit.Id)
            .FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.Equal("Cherry", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFruit_ReturnsUpdatedFruit()
    {
        // Arrange
        var fruit = new Fruit { Name = "Lemon", Id = Guid.NewGuid() };
        await _database.GetCollection<Fruit>("Fruits").InsertOneAsync(fruit);
        fruit.Name = "Updated Lemon";

        // Act & Assert
        var ex = await Record.ExceptionAsync(() => _repository.UpdateAsync(fruit));
        Assert.Null(ex); // Ensure no exception is thrown, indicating success

        var result = await _database.GetCollection<Fruit>("Fruits").Find(x => x.Id == fruit.Id).FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.Equal("Updated Lemon", result.Name);
    }

    [Fact]
    public async Task DeleteAsync_DeletesFruit_FruitNotInCollection()
    {
        // Arrange
        var fruitId = Guid.NewGuid();
        var fruit = new Fruit { Name = "Grape", Id = fruitId };
        await _database.GetCollection<Fruit>("Fruits").InsertOneAsync(fruit);

        // Act
        await _repository.DeleteAsync(fruitId);

        // Assert
        var result = await _database.GetCollection<Fruit>("Fruits").Find(x => x.Id == fruitId).FirstOrDefaultAsync();
        Assert.Null(result);
    }
}