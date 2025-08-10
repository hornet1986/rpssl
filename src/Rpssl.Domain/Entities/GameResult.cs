using Rpssl.Domain.Enums;

namespace Rpssl.Domain.Entities;

public class GameResult
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTimeOffset PlayedAt { get; private set; } = DateTimeOffset.UtcNow;

    public int PlayerChoiceId { get; private set; }
    public int ComputerChoiceId { get; private set; }

    public GameOutcome Outcome { get; private set; }

    public Choice? PlayerChoice { get; private set; }
    public Choice? ComputerChoice { get; private set; }

    private GameResult() { } // EF

    public GameResult(int playerChoiceId, int computerChoiceId, GameOutcome outcome)
    {
        PlayerChoiceId = playerChoiceId;
        ComputerChoiceId = computerChoiceId;
        Outcome = outcome;
    }
}
