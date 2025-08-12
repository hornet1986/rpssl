using MediatR;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Features.Game.Mappings;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Enums;
using Rpssl.Domain.Repositories;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Game.Commands;

internal sealed class PlayGameCommandHandler(
    IChoiceRepository choices,
    IGameResultRepository results,
    IUnitOfWork uow,
    IGameRulesEngine rules
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

        // Pick a random computer choice (based on available choices count)
        IReadOnlyList<Choice> allChoices = await choices.GetAllAsync(cancellationToken);
        if (allChoices.Count == 0)
        {
            return Result.Failure<GameResultDto>(Error.Failure("Choices.Empty", "No choices available"));
        }

        // Simple random selection
        int computerChoiceId = allChoices[new Random().Next(0, allChoices.Count)].Id;

        GameOutcome outcome = rules.DecideOutcome(request.PlayerChoiceId, computerChoiceId);

        GameResult entity = new(request.PlayerChoiceId, computerChoiceId, outcome);
        await results.AddAsync(entity, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.ToDto());
    }
}
