using System;
using System.Threading.Tasks;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Infrastructure.Data.Contexts;
using AwesomeFruits.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AwesomeFruits.Infrastructure.Unit.Tests.Repositories;

public class SqlFruitRepositoryTests
{
    private SqlDbContext GetDbContext(string dbName)
    {
        // Setup options for in-memory database
        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        // Create instance of DbContext
        var dbContext = new SqlDbContext(options);

        // Ensure the database is created
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    private async Task<Fruit> SeedFruitAsync(SqlDbContext dbContext)
    {
        var fruit = new Fruit { Id = Guid.NewGuid(), Name = "Apple", Description = "Red fruit" };
        dbContext.Fruits.Add(fruit);
        await dbContext.SaveChangesAsync();
        return fruit;
    }

    [Fact]
    public async Task FindAllAsync_ReturnsAllFruits()
    {
        using (var dbContext = GetDbContext(nameof(FindAllAsync_ReturnsAllFruits)))
        {
            // Arrange
            await SeedFruitAsync(dbContext);

            var repository = new SqlFruitRepository(dbContext);

            // Act
            var fruits = await repository.FindAllAsync();

            // Assert
            Assert.Single(fruits);
        }
    }

    [Fact]
    public async Task FindByIdAsync_ReturnsFruit_WhenFruitExists()
    {
        using (var dbContext = GetDbContext(nameof(FindByIdAsync_ReturnsFruit_WhenFruitExists)))
        {
            // Arrange
            var seededFruit = await SeedFruitAsync(dbContext);

            var repository = new SqlFruitRepository(dbContext);

            // Act
            var fruit = await repository.FindByIdAsync(seededFruit.Id);

            // Assert
            Assert.NotNull(fruit);
            Assert.Equal(seededFruit.Id, fruit.Id);
        }
    }

    [Fact]
    public async Task SaveAsync_SavesAndReturnsFruit()
    {
        using (var dbContext = GetDbContext(nameof(SaveAsync_SavesAndReturnsFruit)))
        {
            // Arrange
            var fruit = new Fruit { Id = Guid.NewGuid(), Name = "Banana", Description = "Yellow fruit" };

            var repository = new SqlFruitRepository(dbContext);

            // Act
            var savedFruit = await repository.SaveAsync(fruit);

            // Assert
            Assert.NotNull(savedFruit);
            Assert.Equal(fruit.Name, savedFruit.Name);
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesFruit()
    {
        using (var dbContext = GetDbContext(nameof(UpdateAsync_UpdatesFruit)))
        {
            // Arrange
            var fruit = await SeedFruitAsync(dbContext);
            fruit.Name = "Updated Name";
            fruit.Description = "Updated Description";

            var repository = new SqlFruitRepository(dbContext);

            // Act
            var updatedFruit = await repository.UpdateAsync(fruit);

            // Assert
            Assert.NotNull(updatedFruit);
            Assert.Equal("Updated Name", updatedFruit.Name);
            Assert.Equal("Updated Description", updatedFruit.Description);
        }
    }

    [Fact]
    public async Task DeleteAsync_DeletesFruit()
    {
        using (var dbContext = GetDbContext(nameof(DeleteAsync_DeletesFruit)))
        {
            // Arrange
            var fruit = await SeedFruitAsync(dbContext);

            var repository = new SqlFruitRepository(dbContext);

            // Act
            await repository.DeleteAsync(fruit.Id);

            // Assert
            Assert.DoesNotContain(await dbContext.Fruits.ToListAsync(), f => f.Id == fruit.Id);
        }
    }
}