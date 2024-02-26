using System.Threading.Tasks;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Application.Interfaces;
using AwesomeFruits.Domain.Exceptions;
using AwesomeFruits.WebAPI.Users.Controllers;
using AwesomeFruits.WebAPI.Users.Exceptions;
using AwesomeFruits.WebAPI.Users.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace AwesomeFruits.WebAPI.Users.Unit.Tests.Controllers;

public class AuthControllerTests
{
    private readonly AuthController _controller;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IUserService> _mockUserService;

    public AuthControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _controller = new AuthController(_mockUserService.Object, _mockConfiguration.Object);

        // Setup configuration for JWT
        _mockConfiguration.SetupGet(c => c["Jwt:Key"]).Returns("YourSecretKeyHereToTest");
        _mockConfiguration.SetupGet(c => c["Jwt:Issuer"]).Returns("YourIssuer");
        _mockConfiguration.SetupGet(c => c["Jwt:Audience"]).Returns("YourAudience");
    }

    [Fact]
    public async Task Register_ReturnsCreatedResult_WithSaveUserDto()
    {
        // Arrange
        var saveUserDto = new SaveUserDto
        {
            UserName = "testuser", Password = "testpassword", Email = "testemail", FirstName = "FirstName",
            LastName = "LastName"
        };
        _mockUserService.Setup(s => s.SaveUserAsync(It.IsAny<SaveUserDto>())).ReturnsAsync(new UserDto());

        // Act
        var result = await _controller.Register(saveUserDto);

        // Assert
        Assert.IsType<CreatedResult>(result.Result);
    }

    [Fact]
    public async Task Register_UserNameAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var saveUserDto = new SaveUserDto
        {
            UserName = "testuser",
            Password = "testpassword",
            Email = "testemail",
            FirstName = "FirstName",
            LastName = "LastName"
        };
        _mockUserService.Setup(x => x.SaveUserAsync(saveUserDto))
            .ThrowsAsync(new UserNameAlreadyExistsException("Username already exists."));

        // Act & Assert
        await Assert.ThrowsAsync<UserNameAlreadyExistsException>(() => _controller.Register(saveUserDto));
    }

    [Fact]
    public async Task Register_ThrowsValidationErrorsException_WhenValidationFails()
    {
        // Arrange
        var invalidSaveUserDto = new SaveUserDto();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorsException>(() => _controller.Register(invalidSaveUserDto));
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WithValidToken_WhenCredentialsAreValid()
    {
        // Arrange
        var loginUserDto = new LoginUserDto { UserName = "validUser", Password = "validPassword" };
        _mockUserService.Setup(s => s.GetValidLoggedInUser(It.IsAny<LoginUserDto>()))
            .ReturnsAsync(new UserDto { UserName = loginUserDto.UserName });

        // Act
        var result = await _controller.Login(loginUserDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(okResult.Value as Token);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginUserDto = new LoginUserDto { UserName = "invalidUser", Password = "invalidPassword" };
        _mockUserService.Setup(s => s.GetValidLoggedInUser(It.IsAny<LoginUserDto>()))
            .ThrowsAsync(new UserCredentialsNotValidException());

        // Act & Assert
        await Assert.ThrowsAsync<UserCredentialsNotValidException>(() => _controller.Login(loginUserDto));
    }
}