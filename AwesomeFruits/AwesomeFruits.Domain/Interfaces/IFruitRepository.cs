using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeFruits.Domain.Entities;

namespace AwesomeFruits.Domain.Interfaces;

public interface IFruitRepository
{
    Task<IEnumerable<Fruit>> FindAllAsync();
    Task<Fruit> FindByIdAsync(Guid id);
    Task<Fruit> FindByNameAsync(string fruitName);
    Task<Fruit> SaveAsync(Fruit fruit);
    Task<Fruit> UpdateAsync(Fruit fruit);
    Task DeleteAsync(Guid id);
}