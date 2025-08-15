using Rpssl.Application.GameResults;
using Rpssl.Domain.GameResults;

namespace Rpssl.Application.Games.Mappings;

public static class GameMappings
{
    public static GameResultDto ToDto(this GameResult entity) => new(
        entity.PlayerChoiceId,
        entity.ComputerChoiceId,
        entity.Outcome
    );
}
