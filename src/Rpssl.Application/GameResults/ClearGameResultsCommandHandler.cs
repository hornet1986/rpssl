using Rpssl.Application.Abstractions;
using Rpssl.Domain.GameResults;
using Rpssl.Domain.UnitOfWork;
using Rpssl.SharedKernel;

namespace Rpssl.Application.GameResults;

internal sealed class ClearGameResultsCommandHandler(
    IGameResultRepository repo,
    IUnitOfWork uow
) : IRequestHandler<ClearGameResultsCommand, Result>
{
    public async Task<Result> Handle(ClearGameResultsCommand request, CancellationToken cancellationToken)
    {
        await repo.DeleteAllAsync(cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
