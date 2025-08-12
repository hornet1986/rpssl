using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Rpssl.Domain.Repositories;

namespace Rpssl.Infrastructure.Database.UnitOfWork;

public class EfUnitOfWork(RpsslDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => dbContext.SaveChangesAsync(ct);

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> work, CancellationToken ct = default)
    {
        // Use explicit transaction to guarantee atomicity across multiple SaveChanges (if any) inside work
        IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction tx = await dbContext.Database.BeginTransactionAsync(ct);
            await work(ct);
            await dbContext.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        });
    }
}
