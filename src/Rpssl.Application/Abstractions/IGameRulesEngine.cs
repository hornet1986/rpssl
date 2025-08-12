using Rpssl.Domain.Enums;

namespace Rpssl.Application.Abstractions;

public interface IGameRulesEngine
{
    // Returns the game outcome for player vs computer
    GameOutcome DecideOutcome(int playerChoiceId, int computerChoiceId);
}
