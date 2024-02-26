using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwesomeFruits.Domain.Entities;
using AwesomeFruits.Domain.Interfaces;
using AwesomeFruits.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AwesomeFruits.Infrastructure.Data.Repositories;

public class SqlFruitRepository : IFruitRepository
{
    private readonly SqlDbContext _context;

    public SqlFruitRepository(SqlDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Fruit>> FindAllAsync()
    {
        return await _context.Fruits
            .ToListAsync();
    }

    public async Task<Fruit> FindByNameAsync(string fruitName)
    {
        return await _context.Fruits.Where(x => x.Name == fruitName).FirstOrDefaultAsync();
    }

    public async Task<Fruit> FindByIdAsync(Guid id)
    {
        return await _context.Fruits
            .FindAsync(id);
    }

    public async Task<Fruit> SaveAsync(Fruit fruit)
    {
        _context.Fruits.Add(fruit);
        await _context.SaveChangesAsync();

        return fruit;
    }

    public async Task<Fruit> UpdateAsync(Fruit fruit)
    {
        var newFruit = await _context.Fruits.FindAsync(fruit.Id);

        newFruit.Name = fruit.Name;
        newFruit.Description = fruit.Description;
        newFruit.LastUpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return newFruit;
    }

    public async Task DeleteAsync(Guid id)
    {
        var fruit = await _context.Fruits.FindAsync(id);
        _context.Fruits.Remove(fruit);
        await _context.SaveChangesAsync();
    }
}