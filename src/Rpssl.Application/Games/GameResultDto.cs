using Rpssl.Domain.GameOutcome;

namespace Rpssl.Application.Games;

public sealed record GameResultDto(
    int PlayerChoiceId,
    int ComputerChoiceId,
    GameOutcome Outcome
);
