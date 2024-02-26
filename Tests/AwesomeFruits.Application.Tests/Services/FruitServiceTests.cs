using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Application.Services;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Domain.Exceptions;
using AwesomeFruits.Domain.Interfaces;
using Moq;
using Xunit;

namespace AwesomeFruits.Application.Unit.Tests.Services;

public class FruitServiceTests
{
    private readonly FruitService _fruitService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IFruitRepository> _mockRepo;

    public FruitServiceTests()
    {
        _mockRepo = new Mock<IFruitRepository>();
        _mockMapper = new Mock<IMapper>();
        _fruitService = new FruitService(_mockRepo.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task FindAllFruitsAsync_ReturnsAllFruits()
    {
        // Arrange

        var fruits = new List<Fruit>
        {
            new() { Id = Guid.NewGuid(), Name = "Apple", Description = "Red fruit" },
            new() { Id = Guid.NewGuid(), Name = "Banana", Description = "Yellow fruit" }
        };
        var fruitDtos = new List<FruitDto>
        {
            new() { Id = fruits[0].Id, Name = "Apple", Description = "Red fruit" },
            new() { Id = fruits[1].Id, Name = "Banana", Description = "Yellow fruit" }
        };

        _mockRepo.Setup(repo => repo.FindAllAsync()).ReturnsAsync(fruits);
        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<FruitDto>>(It.IsAny<IEnumerable<Fruit>>()))
            .Returns(fruitDtos);

        // Act
        var result = await _fruitService.FindAllFruitsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepo.Verify(repo => repo.FindAllAsync(), Times.Once);
        _mockMapper.Verify(mapper => mapper.Map<IEnumerable<FruitDto>>(fruits), Times.Once);
    }

    [Fact]
    public async Task FindFruitByIdAsync_ReturnsFruitDto_IfExists()
    {
        // Arrange
        var fruit = new Fruit { Id = Guid.NewGuid(), Name = "Apple", Description = "Red fruit" };
        var fruitDto = new FruitDto { Id = fruit.Id, Name = fruit.Name, Description = fruit.Description };
        _mockRepo.Setup(repo => repo.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(fruit);
        _mockMapper.Setup(mapper => mapper.Map<FruitDto>(It.IsAny<Fruit>())).Returns(fruitDto);

        // Act
        var result = await _fruitService.FindFruitByIdAsync(fruit.Id);

        // Assert
        Assert.Equal(fruitDto.Name, result.Name);
        Assert.Equal(fruitDto.Description, result.Description);
    }

    [Fact]
    public async Task FindFruitByIdAsync_ThrowsEntityNotFoundException_WhenFruitNotFound()
    {
        // Arrange
        var fruitId = Guid.NewGuid();
        _mockRepo.Setup(repo => repo.FindByIdAsync(fruitId)).ReturnsAsync((Fruit)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _fruitService.FindFruitByIdAsync(fruitId));
    }

    [Fact]
    public async Task SaveFruitAsync_SavesAndReturnsFruitDto()
    {
        // Arrange
        var saveFruitDto = new SaveFruitDto { Name = "Banana", Description = "Yellow fruit" };
        var savedFruit = new Fruit
            { Id = Guid.NewGuid(), Name = saveFruitDto.Name, Description = saveFruitDto.Description };
        var fruitDto = new FruitDto
            { Id = savedFruit.Id, Name = savedFruit.Name, Description = savedFruit.Description };

        _mockMapper.Setup(m => m.Map<Fruit>(It.IsAny<SaveFruitDto>())).Returns(savedFruit);
        _mockRepo.Setup(repo => repo.SaveAsync(It.IsAny<Fruit>())).ReturnsAsync(savedFruit);
        _mockMapper.Setup(m => m.Map<FruitDto>(It.IsAny<Fruit>())).Returns(fruitDto);

        // Act
        var result = await _fruitService.SaveFruitAsync(saveFruitDto);

        // Assert
        Assert.Equal(fruitDto, result);
    }

    [Fact]
    public async Task UpdateFruitAsync_UpdatesAndReturnsFruitDto()
    {
        // Arrange
        var updateFruitDto = new UpdateFruitDto
            { Id = Guid.NewGuid(), Name = "Strawberry", Description = "Red and sweet" };
        var updatedFruit = new Fruit
            { Id = updateFruitDto.Id, Name = updateFruitDto.Name, Description = updateFruitDto.Description };
        var fruitDto = new FruitDto
            { Id = updatedFruit.Id, Name = updatedFruit.Name, Description = updatedFruit.Description };

        _mockRepo.Setup(repo => repo.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(updatedFruit);
        _mockMapper.Setup(m => m.Map<Fruit>(It.IsAny<UpdateFruitDto>())).Returns(updatedFruit);
        _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Fruit>())).ReturnsAsync(updatedFruit);
        _mockMapper.Setup(m => m.Map<FruitDto>(It.IsAny<Fruit>())).Returns(fruitDto);

        // Act
        var result = await _fruitService.UpdateFruitAsync(updateFruitDto);

        // Assert
        Assert.Equal(fruitDto, result);
    }

    [Fact]
    public async Task UpdateFruitAsync_ThrowsEntityNotFoundException_WhenFruitNotFound()
    {
        // Arrange
        var updateFruitDto = new UpdateFruitDto
            { Id = Guid.NewGuid(), Name = "Strawberry", Description = "Red and sweet" };

        _mockRepo.Setup(repo => repo.FindByIdAsync(updateFruitDto.Id)).ReturnsAsync((Fruit)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _fruitService.UpdateFruitAsync(updateFruitDto));
    }

    [Fact]
    public async Task DeleteFruitAsync_DeletesFruit_IfExists()
    {
        // Arrange
        var fruitId = Guid.NewGuid();
        var fruit = new Fruit { Id = fruitId, Name = "Grape", Description = "Small and purple" };

        _mockRepo.Setup(repo => repo.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(fruit);
        _mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        // Act & Assert
        await _fruitService.DeleteFruitAsync(fruitId);

        _mockRepo.Verify(repo => repo.DeleteAsync(fruitId), Times.Once);
    }

    [Fact]
    public async Task DeleteFruitAsync_ThrowsEntityNotFoundException_WhenFruitNotFound()
    {
        // Arrange
        var fruitId = Guid.NewGuid();

        _mockRepo.Setup(repo => repo.FindByIdAsync(fruitId)).ReturnsAsync((Fruit)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _fruitService.DeleteFruitAsync(fruitId));
    }
}