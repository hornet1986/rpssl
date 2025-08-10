using Microsoft.EntityFrameworkCore;
using Rpssl.Domain.Entities;

namespace Rpssl.Infrastructure.Database;

public class RpsslDbContext(DbContextOptions<RpsslDbContext> options) : DbContext(options)
{
    public DbSet<Choice> Choices { get; set; }
    public DbSet<GameResult> GameResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RpsslDbContext).Assembly);
    }
}
