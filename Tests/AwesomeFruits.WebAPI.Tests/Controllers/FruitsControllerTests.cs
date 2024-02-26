using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Application.Interfaces;
using AwesomeFruits.Domain.Exceptions;
using AwesomeFruits.WebAPI.Controllers;
using AwesomeFruits.WebAPI.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AwesomeFruits.WebAPI.Unit.Tests.Controllers;

public class FruitsControllerTests
{
    private readonly FruitsController _controller;
    private readonly Mock<IFruitService> _mockFruitService;
    private readonly Mock<HttpContext> _mockHttpContext;

    public FruitsControllerTests()
    {
        _mockFruitService = new Mock<IFruitService>();
        _controller = new FruitsController(_mockFruitService.Object);

        _mockHttpContext = new Mock<HttpContext>();

        // Setup user identity
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _mockHttpContext.Setup(m => m.User).Returns(claimsPrincipal);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _mockHttpContext.Object
        };
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WithAllFruits()
    {
        // Arrange
        _mockFruitService.Setup(s => s.FindAllFruitsAsync()).ReturnsAsync(new List<FruitDto>());

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithFruit()
    {
        // Arrange
        var fruitId = Guid.NewGuid();
        _mockFruitService.Setup(s => s.FindFruitByIdAsync(fruitId)).ReturnsAsync(new FruitDto());

        // Act
        var result = await _controller.Get(fruitId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ThrowsEntityNotFoundException_WhenFruitNotFound()
    {
        // Arrange
        var fruitId = Guid.NewGuid();
        _mockFruitService.Setup(s => s.FindFruitByIdAsync(fruitId)).ThrowsAsync(new EntityNotFoundException());

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _controller.Get(fruitId));
    }

    [Fact]
    public async Task Post_ReturnsCreatedResult_WithSaveFruitDto()
    {
        // Arrange

        var fruitName = "TestName";
        var saveFruitDto = new SaveFruitDto
        {
            Name = fruitName,
            Description = "TestDesc"
        };

        var fruitDto = new FruitDto
        {
            Name = fruitName
        };
        _mockFruitService.Setup(s => s.SaveFruitAsync(saveFruitDto)).ReturnsAsync(fruitDto);

        // Act
        var result = await _controller.Post(saveFruitDto);

        // Assert
        Assert.IsType<CreatedResult>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Post_Fruit_ThrowsFruitAlreadyExistsException_WhenFruitAlreadyExists()
    {
        // Arrange
        var saveFruitDto = new SaveFruitDto
        {
            Name = "TestName",
            Description = "TestDesc"
        };
        _mockFruitService.Setup(x => x.SaveFruitAsync(saveFruitDto))
            .ThrowsAsync(new FruitAlreadyExistsException("Fruit already exists."));

        // Act & Assert
        await Assert.ThrowsAsync<FruitAlreadyExistsException>(() => _controller.Post(saveFruitDto));
    }

    [Fact]
    public async Task Post_ValidationErrors_ThrowsValidationErrorsException()
    {
        // Arrange
        var saveFruitDto = new SaveFruitDto
        {
            /* Initialize with invalid data */
        };
        _mockFruitService.Setup(x => x.SaveFruitAsync(It.IsAny<SaveFruitDto>()))
            .ThrowsAsync(new ValidationErrorsException(new List<string> { "validation error" }));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorsException>(() => _controller.Post(saveFruitDto));
    }

    [Fact]
    public async Task Post_Fruit_ThrowsGeneralException_WhenUnexpectedError()
    {
        // Arrange
        var saveFruitDto = new SaveFruitDto
        {
            Name = "TestName",
            Description = "TestDesc"
        };
        _mockFruitService.Setup(x => x.SaveFruitAsync(saveFruitDto))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.Post(saveFruitDto));
    }

    [Fact]
    public async Task Put_ReturnsNoContentResult_WhenFruitUpdated()
    {
        // Arrange
        var updateFruitDto = new UpdateFruitDto
        {
            Description = "TestDe",
            Id = Guid.NewGuid(),
            Name = "TestName"
        };
        _mockFruitService.Setup(s => s.UpdateFruitAsync(It.IsAny<UpdateFruitDto>())).ReturnsAsync(new FruitDto());

        // Act
        var result = await _controller.Put(updateFruitDto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Put_Fruit_ThrowsFruitAlreadyExistsException_WhenFruitAlreadyExists()
    {
        // Arrange
        var updateFruitDto = new UpdateFruitDto
        {
            Name = "TestName",
            Description = "TestDesc"
        };
        _mockFruitService.Setup(x => x.UpdateFruitAsync(updateFruitDto))
            .ThrowsAsync(new FruitAlreadyExistsException("Fruit already exists."));

        // Act & Assert
        await Assert.ThrowsAsync<FruitAlreadyExistsException>(() => _controller.Put(updateFruitDto));
    }

    [Fact]
    public async Task Put_ValidationErrors_ThrowsValidationErrorsException()
    {
        // Arrange
        var updateFruitDto = new UpdateFruitDto();
        _mockFruitService.Setup(x => x.UpdateFruitAsync(It.IsAny<UpdateFruitDto>()))
            .ThrowsAsync(new ValidationErrorsException(new List<string> { "validation error" }));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationErrorsException>(() => _controller.Put(updateFruitDto));
    }

    [Fact]
    public async Task Put_Fruit_ThrowsGeneralException_WhenUnexpectedError()
    {
        // Arrange
        var updateFruitDto = new UpdateFruitDto
        {
            Name = "TestName",
            Description = "TestDesc"
        };
        _mockFruitService.Setup(x => x.UpdateFruitAsync(updateFruitDto))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.Put(updateFruitDto));
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResult_WhenFruitDeleted()
    {
        // Arrange
        var fruitId = Guid.NewGuid();
        _mockFruitService.Setup(s => s.DeleteFruitAsync(fruitId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(fruitId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}