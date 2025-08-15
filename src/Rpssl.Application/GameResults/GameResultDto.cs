using Rpssl.Domain.GameOutcome;

namespace Rpssl.Application.GameResults;

public sealed record GameResultDto(
    int PlayerChoiceId,
    int ComputerChoiceId,
    GameOutcome Outcome
);
