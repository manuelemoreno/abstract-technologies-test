using AwesomeFruits.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AwesomeFruits.Infrastructure.Data.Contexts;

public class SqlDbContext : DbContext
{
    public SqlDbContext(DbContextOptions<SqlDbContext> options)
        : base(options)
    {
    }

    public DbSet<Fruit> Fruits { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fruit>()
            .Property(e => e.Name).HasMaxLength(255);

        modelBuilder.Entity<Fruit>()
            .Property(e => e.Description).HasMaxLength(500);
    }
}