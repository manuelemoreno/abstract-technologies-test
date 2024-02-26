using AwesomeFruits.Domain.Entities;
using MongoDB.Driver;

namespace AwesomeFruits.Infrastructure.Data.Contexts;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Fruit> Fruits => _database.GetCollection<Fruit>("Fruits");
    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
}