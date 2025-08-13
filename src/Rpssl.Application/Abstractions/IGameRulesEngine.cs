using Rpssl.Domain.Enums;

namespace Rpssl.Application.Abstractions;

public interface IGameRulesEngine
{
    /// <summary>
    /// Determines the outcome of a round for the player against the computer.
    /// </summary>
    /// <param name="playerChoiceId">The player's selected gesture identifier.</param>
    /// <param name="computerChoiceId">The computer's selected gesture identifier.</param>
    /// <returns>The <see cref="GameOutcome"/> representing win, lose or draw for the player.</returns>
    GameOutcome DecideOutcome(int playerChoiceId, int computerChoiceId);
}
