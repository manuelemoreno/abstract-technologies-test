using System;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Application.Services;
using AwesomeFruits.Application.Utilities;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Domain.Exceptions;
using AwesomeFruits.Domain.Interfaces;
using Moq;
using Xunit;

namespace AwesomeFruits.Application.Unit.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetValidLoggedInUser_ReturnsUserDto_WhenCredentialsAreValid()
    {
        // Arrange
        var loginUserDto = new LoginUserDto { UserName = "testUser", Password = "testPassword" };
        var (hashedPassword, salt) = PasswordHasher.HashPassword(loginUserDto.Password);

        var user = new User
        {
            UserName = loginUserDto.UserName, PasswordHash = hashedPassword,
            PasswordSalt = Convert.ToBase64String(salt)
        };
        var userDto = new UserDto { UserName = user.UserName };

        _mockUserRepository.Setup(x => x.FindByUserNameAsync(loginUserDto.UserName)).ReturnsAsync(user);
        _mockMapper.Setup(x => x.Map<UserDto>(It.IsAny<User>())).Returns(userDto);

        // Act
        var result = await _userService.GetValidLoggedInUser(loginUserDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userDto.UserName, result.UserName);
    }

    [Fact]
    public async Task GetValidLoggedInUser_ThrowsUserCredentialsNotValidException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginUserDto = new LoginUserDto { UserName = "testUser", Password = "wrongPassword" };
        var user = new User
        {
            UserName = "testUser", PasswordHash = "hashedPassword",
            PasswordSalt = Convert.ToBase64String(new byte[] { 1, 2, 3 })
        };

        _mockUserRepository.Setup(x => x.FindByUserNameAsync(loginUserDto.UserName)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UserCredentialsNotValidException>(() =>
            _userService.GetValidLoggedInUser(loginUserDto));
    }
}