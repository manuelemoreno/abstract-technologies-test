using System.Threading.Tasks;
using AwesomeFruits.Domain.Entities;

namespace AwesomeFruits.Domain.Interfaces;

public interface IUserRepository
{
    Task<User> FindByUserNameAsync(string userName);
    Task<User> SaveAsync(User user);
}