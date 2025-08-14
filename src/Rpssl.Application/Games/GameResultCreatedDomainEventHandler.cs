using Rpssl.Domain.GameResults;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Games;

internal sealed class GameResultCreatedDomainEventHandler : IDomainEventHandler<GameResultCreatedDomainEvent>
{
    public Task Handle(GameResultCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // TODO: Send an email with game results, etc.
        return Task.CompletedTask;
    }
}
