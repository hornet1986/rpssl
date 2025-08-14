using Rpssl.Domain.UnitOfWork;
using Rpssl.Infrastructure.DomainEvents;
using Rpssl.SharedKernel;

namespace Rpssl.Infrastructure.Database.UnitOfWork;

public class EfUnitOfWork(RpsslDbContext dbContext, IDomainEventsDispatcher domainEventsDispatcher) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        int result = await dbContext.SaveChangesAsync(ct);
        await PublishDomainEventsAsync();
        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        // Gather all domain events from tracked entities, then clear them before dispatching
        var domainEvents = dbContext.ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .SelectMany(entity =>
            {
                var events = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();
                return events;
            })
            .ToList();

        if (domainEvents.Count == 0)
        {
            return;
        }

        await domainEventsDispatcher.DispatchAsync(domainEvents);
    }
}
