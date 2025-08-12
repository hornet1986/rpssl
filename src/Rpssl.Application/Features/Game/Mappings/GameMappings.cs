using Rpssl.Domain.Entities;

namespace Rpssl.Application.Features.Game.Mappings;

public static class GameMappings
{
    public static GameResultDto ToDto(this GameResult entity) => new(
        entity.PlayerChoiceId,
        entity.ComputerChoiceId,
        entity.Outcome
    );
}
