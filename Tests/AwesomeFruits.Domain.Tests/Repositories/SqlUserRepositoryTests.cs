using System.Threading.Tasks;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Infrastructure.Data.Contexts;
using AwesomeFruits.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AwesomeFruits.Infrastructure.Unit.Tests.Repositories;

public class SqlUserRepositoryTests
{
    private readonly DbContextOptions<SqlDbContext> _dbContextOptions;

    public SqlUserRepositoryTests()
    {
        // Setup in-memory database
        _dbContextOptions = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase("TestUserDb")
            .Options;

        // Ensure the database is created and seeded with test data if necessary
        using (var context = new SqlDbContext(_dbContextOptions))
        {
            // Seed database with test data if necessary
        }
    }

    [Fact]
    public async Task FindByUserNameAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var userName = "testUser";
        var user = new User { UserName = userName, IsActive = true };

        using (var arrangeContext = new SqlDbContext(_dbContextOptions))
        {
            arrangeContext.Users.Add(user);
            await arrangeContext.SaveChangesAsync();
        }

        // Act
        User foundUser;
        using (var actContext = new SqlDbContext(_dbContextOptions))
        {
            var repository = new SqlUserRepository(actContext);
            foundUser = await repository.FindByUserNameAsync(userName);
        }

        // Assert
        Assert.NotNull(foundUser);
        Assert.Equal(userName, foundUser.UserName);
    }

    [Fact]
    public async Task SaveAsync_PersistsUserCorrectly()
    {
        // Arrange
        var user = new User { UserName = "newUser", IsActive = false }; // IsActive should be overridden to true

        // Act
        using (var actContext = new SqlDbContext(_dbContextOptions))
        {
            var repository = new SqlUserRepository(actContext);
            var savedUser = await repository.SaveAsync(user);

            // Assert within act block for simplicity, although could be separated
            Assert.True(savedUser.IsActive);
        }

        // Assert that user is saved in the database
        using (var assertContext = new SqlDbContext(_dbContextOptions))
        {
            var userInDb = await assertContext.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            Assert.NotNull(userInDb);
            Assert.True(userInDb.IsActive);
        }
    }
}