using System.Threading.Tasks;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Domain.Interfaces;
using AwesomeFruits.Infrastructure.Data.Contexts;
using MongoDB.Driver;

namespace AwesomeFruits.Infrastructure.Data.Repositories;

public class MongoUserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public MongoUserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<User> FindByUserNameAsync(string userName)
    {
        return await _context.Users.Find(x => x.UserName == userName)
            .FirstOrDefaultAsync();
    }

    public async Task<User> SaveAsync(User user)
    {
        user.IsActive = true;
        await _context.Users.InsertOneAsync(user);

        return user;
    }
}