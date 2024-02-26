using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeFruits.Application.DTOs;
using AwesomeFruits.Application.Interfaces;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Domain.Exceptions;
using AwesomeFruits.Domain.Interfaces;

namespace AwesomeFruits.Application.Services;

public class FruitService : IFruitService
{
    private readonly IFruitRepository _fruitRepository;
    private readonly IMapper _mapper;

    public FruitService(IFruitRepository fruitFruitRepository, IMapper mapper)
    {
        _fruitRepository = fruitFruitRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FruitDto>> FindAllFruitsAsync()
    {
        var getFruits = await _fruitRepository.FindAllAsync();

        return _mapper.Map<IEnumerable<FruitDto>>(getFruits);
    }

    public async Task<FruitDto> FindFruitByIdAsync(Guid id)
    {
        var getFruit = await _fruitRepository.FindByIdAsync(id);

        return getFruit == null
            ? throw new EntityNotFoundException()
            : _mapper.Map<FruitDto>(getFruit);
    }

    public async Task<FruitDto> SaveFruitAsync(SaveFruitDto saveFruitDto)
    {
        var getFruit = await _fruitRepository.FindByNameAsync(saveFruitDto.Name);

        if (getFruit != null) throw new FruitAlreadyExistsException();

        var fruit = _mapper.Map<Fruit>(saveFruitDto);

        var saveFruit = await _fruitRepository.SaveAsync(fruit);

        return _mapper.Map<FruitDto>(saveFruit);
    }

    public async Task<FruitDto> UpdateFruitAsync(UpdateFruitDto updateFruitDto)
    {
        var getFruit = await _fruitRepository.FindByNameAsync(updateFruitDto.Name);

        if (getFruit != null) throw new FruitAlreadyExistsException();

        var fruit = _mapper.Map<Fruit>(updateFruitDto);

        var currentFruit = await FindFruitByIdAsync(updateFruitDto.Id);

        fruit.LastUpdatedAtUtc = DateTime.UtcNow;
        fruit.CreatedAtUtc = currentFruit.CreatedAtUtc;
        fruit.CreatedByUserId = currentFruit.CreatedByUserId;

        var updateFruit = await _fruitRepository.UpdateAsync(fruit);

        return _mapper.Map<FruitDto>(updateFruit);
    }

    public async Task DeleteFruitAsync(Guid id)
    {
        await FindFruitByIdAsync(id);

        await _fruitRepository.DeleteAsync(id);
    }
}