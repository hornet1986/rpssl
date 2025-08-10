using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Rpssl.Domain.Repositories;

namespace Rpssl.Infrastructure.Database.UnitOfWork;

public class EfUnitOfWork(RpsslDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> work, CancellationToken ct = default)
    {
        // Use explicit transaction to guarantee atomicity across multiple SaveChanges (if any) inside work
        IExecutionStrategy strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction tx = await db.Database.BeginTransactionAsync(ct);
            await work(ct);
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        });
    }
}
