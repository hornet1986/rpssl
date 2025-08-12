using Rpssl.Domain.Enums;

namespace Rpssl.Application.Features.Game;

public sealed record GameResultDto(
    int PlayerChoiceId,
    int ComputerChoiceId,
    GameOutcome Outcome
);
