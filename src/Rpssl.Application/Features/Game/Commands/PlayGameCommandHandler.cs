using Rpssl.Application.Features.Game.Mappings;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Enums;
using Rpssl.Domain.Repositories;
using Rpssl.SharedKernel;
using Rpssl.Application.Abstractions;

namespace Rpssl.Application.Features.Game.Commands;

internal sealed class PlayGameCommandHandler(
    IChoiceRepository choices,
    IGameResultRepository results,
    IUnitOfWork uow,
    IGameRulesEngine rules,
    IRandomNumberService randomNumberService
) : IRequestHandler<PlayGameCommand, Result<GameResultDto>>
{
    private static readonly Error PlayerChoiceNotFound = Error.NotFound(
        "Choices.NotFound",
        "Player choice not found");

    public async Task<Result<GameResultDto>> Handle(PlayGameCommand request, CancellationToken cancellationToken)
    {
        // Validate player choice exists
        Choice? playerChoice = await choices.GetByIdAsync(request.PlayerChoiceId, cancellationToken);
        if (playerChoice is null)
        {
            return Result.Failure<GameResultDto>(PlayerChoiceNotFound);
        }

        // Get a random number from remote service (1-5)
        Result<int> randomResult = await randomNumberService.GetRandomOneToFiveAsync(cancellationToken);
        if (randomResult.IsFailure)
        {
            return Result.Failure<GameResultDto>(randomResult.Error);
        }
        int computerChoiceId = randomResult.Value;

        GameOutcome outcome = rules.DecideOutcome(request.PlayerChoiceId, computerChoiceId);

        GameResult entity = new(request.PlayerChoiceId, computerChoiceId, outcome);
        await results.AddAsync(entity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}
