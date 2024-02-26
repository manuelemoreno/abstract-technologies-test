using System.Linq;
using System.Threading.Tasks;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Domain.Interfaces;
using AwesomeFruits.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AwesomeFruits.Infrastructure.Data.Repositories;

public class SqlUserRepository : IUserRepository
{
    private readonly SqlDbContext _context;

    public SqlUserRepository(SqlDbContext context)
    {
        _context = context;
    }

    public async Task<User> FindByUserNameAsync(string userName)
    {
        return await _context.Users
            .Where(x => x.UserName == userName).FirstOrDefaultAsync();
    }

    public async Task<User> SaveAsync(User user)
    {
        user.IsActive = true;
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}