using FluentValidation.Results;
using Rpssl.Application.Games;

namespace Rpssl.Application.Tests.Game;

[TestClass]
public class PlayGameCommandValidatorTests
{
    [TestMethod]
    public void PlayerChoiceId_MustBeBetween1And5()
    {
        var validator = new PlayGameCommandValidator();

        ValidationResult r1 = validator.Validate(new PlayGameCommand(0));
        Assert.IsTrue(!r1.IsValid && r1.Errors.Any(e => e.PropertyName == nameof(PlayGameCommand.PlayerChoiceId)));

        ValidationResult r2 = validator.Validate(new PlayGameCommand(6));
        Assert.IsTrue(!r2.IsValid && r2.Errors.Any(e => e.PropertyName == nameof(PlayGameCommand.PlayerChoiceId)));

        ValidationResult ok1 = validator.Validate(new PlayGameCommand(1));
        Assert.IsTrue(ok1.IsValid);

        ValidationResult ok2 = validator.Validate(new PlayGameCommand(5));
        Assert.IsTrue(ok2.IsValid);
    }
}
