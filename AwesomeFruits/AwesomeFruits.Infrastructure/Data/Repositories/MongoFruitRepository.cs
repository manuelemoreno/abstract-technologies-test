using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Domain.Interfaces;
using AwesomeFruits.Infrastructure.Data.Contexts;
using MongoDB.Driver;

namespace AwesomeFruits.Infrastructure.Data.Repositories;

public class MongoFruitRepository : IFruitRepository
{
    private readonly MongoDbContext _context;

    public MongoFruitRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Fruit>> FindAllAsync()
    {
        return await _context.Fruits.Find(_ => true).ToListAsync();
    }

    public async Task<Fruit> FindByNameAsync(string fruitName)
    {
        return await _context.Fruits.Find(x => x.Name == fruitName).FirstOrDefaultAsync();
    }

    public async Task<Fruit> FindByIdAsync(Guid id)
    {
        return await _context.Fruits.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Fruit> SaveAsync(Fruit fruit)
    {
        await _context.Fruits.InsertOneAsync(fruit);

        return fruit;
    }

    public async Task<Fruit> UpdateAsync(Fruit fruit)
    {
        var result = await _context.Fruits.ReplaceOneAsync(x => x.Id == fruit.Id, fruit);
        if (result.IsAcknowledged && result.ModifiedCount > 0)
            // The update was successful
            return fruit;

        throw new InvalidOperationException("Update operation was not acknowledged or no document was modified.");
    }

    public async Task DeleteAsync(Guid id)
    {
        await _context.Fruits.DeleteOneAsync(x => x.Id == id);
    }
}