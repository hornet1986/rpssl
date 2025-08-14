using Rpssl.Application.Abstractions;
using Rpssl.Domain.GameOutcome;

namespace Rpssl.Application.Services;

// Rock(1) Paper(2) Scissors(3) Lizard(4) Spock(5)
public sealed class GameRulesEngine : IGameRulesEngine
{
    public GameOutcome DecideOutcome(int playerChoiceId, int computerChoiceId)
    {
        if (playerChoiceId == computerChoiceId)
        {
            return GameOutcome.Draw;
        }

        // Rules encoded as winning pairs (A beats B)
        var wins = new HashSet<(int, int)>
        {
            (1,3), // Rock crushes Scissors
            (1,4), // Rock crushes Lizard
            (2,1), // Paper covers Rock
            (2,5), // Paper disproves Spock
            (3,2), // Scissors cuts Paper
            (3,4), // Scissors decapitates Lizard
            (4,2), // Lizard eats Paper
            (4,5), // Lizard poisons Spock
            (5,1), // Spock vaporizes Rock
            (5,3)  // Spock smashes Scissors
        };

        return wins.Contains((playerChoiceId, computerChoiceId)) ? GameOutcome.PlayerWin : GameOutcome.ComputerWin;
    }
}
