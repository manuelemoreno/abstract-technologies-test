using System.Threading.Tasks;
using AwesomeFruits.Application.DTOs;

namespace AwesomeFruits.Application.Interfaces;

public interface IUserService
{
    Task<UserDto> GetValidLoggedInUser(LoginUserDto loginUserDto);
    Task<UserDto> SaveUserAsync(SaveUserDto saveUserDto);
}