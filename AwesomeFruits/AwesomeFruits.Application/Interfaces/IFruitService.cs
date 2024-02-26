using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeFruits.Application.DTOs;

namespace AwesomeFruits.Application.Interfaces;

public interface IFruitService
{
    Task<IEnumerable<FruitDto>> FindAllFruitsAsync();
    Task<FruitDto> FindFruitByIdAsync(Guid id);
    Task<FruitDto> SaveFruitAsync(SaveFruitDto saveFruitDto);
    Task<FruitDto> UpdateFruitAsync(UpdateFruitDto updateFruitDto);
    Task DeleteFruitAsync(Guid id);

    //Search Fruits
}