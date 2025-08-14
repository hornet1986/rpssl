using Rpssl.Application.Services;
using Rpssl.Domain.GameOutcome;

namespace Rpssl.Application.Tests.Services;

[TestClass]
public class GameRulesEngineTests
{
    private readonly GameRulesEngine _engine = new();

    [DataTestMethod]
    [DataRow(1, 1, GameOutcome.Draw)]
    [DataRow(1, 3, GameOutcome.PlayerWin)] // Rock crushes Scissors
    [DataRow(1, 4, GameOutcome.PlayerWin)] // Rock crushes Lizard
    [DataRow(1, 2, GameOutcome.ComputerWin)] // Paper covers Rock
    [DataRow(2, 1, GameOutcome.PlayerWin)] // Paper covers Rock
    [DataRow(2, 5, GameOutcome.PlayerWin)] // Paper disproves Spock
    [DataRow(3, 2, GameOutcome.PlayerWin)] // Scissors cuts Paper
    [DataRow(3, 4, GameOutcome.PlayerWin)] // Scissors decapitates Lizard
    [DataRow(4, 2, GameOutcome.PlayerWin)] // Lizard eats Paper
    [DataRow(4, 5, GameOutcome.PlayerWin)] // Lizard poisons Spock
    [DataRow(5, 1, GameOutcome.PlayerWin)] // Spock vaporizes Rock
    [DataRow(5, 3, GameOutcome.PlayerWin)] // Spock smashes Scissors
    public void DecideOutcome_ReturnsExpected(int player, int computer, GameOutcome expected)
    {
        GameOutcome result = _engine.DecideOutcome(player, computer);
        Assert.AreEqual(expected, result);
    }
}
