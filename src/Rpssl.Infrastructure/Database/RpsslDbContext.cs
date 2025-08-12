using Microsoft.EntityFrameworkCore;
using Rpssl.Domain.Entities;

namespace Rpssl.Infrastructure.Database;

public sealed class RpsslDbContext : DbContext
{
    public RpsslDbContext()
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public RpsslDbContext(DbContextOptions<RpsslDbContext> options) : base(options)
    { }

    public DbSet<Choice> Choices { get; set; }
    public DbSet<GameResult> GameResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RpsslDbContext).Assembly);
    }
}
