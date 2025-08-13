using Rpssl.Domain.Repositories;

namespace Rpssl.Infrastructure.Database.UnitOfWork;

public class EfUnitOfWork(RpsslDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => dbContext.SaveChangesAsync(ct);
}
