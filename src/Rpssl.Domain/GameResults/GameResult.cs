using Rpssl.Domain.Choices;
using Rpssl.SharedKernel;

namespace Rpssl.Domain.GameResults;

public class GameResult(int playerChoiceId, int computerChoiceId, GameOutcome.GameOutcome outcome) : Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public DateTimeOffset PlayedAt { get; private set; } = DateTimeOffset.UtcNow;

    public int PlayerChoiceId { get; private set; } = playerChoiceId;

    public int ComputerChoiceId { get; private set; } = computerChoiceId;

    public GameOutcome.GameOutcome Outcome { get; private set; } = outcome;

    public Choice? PlayerChoice { get; private set; } = null!;

    public Choice? ComputerChoice { get; private set; } = null!;
}
