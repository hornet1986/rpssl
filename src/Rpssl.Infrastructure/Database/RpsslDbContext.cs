using Microsoft.EntityFrameworkCore;
using Rpssl.Domain.Choices;
using Rpssl.Domain.GameResults;
using Rpssl.Domain.RefreshTokens;
using Rpssl.Domain.Users;

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
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RpsslDbContext).Assembly);
    }
}
