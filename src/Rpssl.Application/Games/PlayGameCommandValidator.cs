using FluentValidation;

namespace Rpssl.Application.Games;

public sealed class PlayGameCommandValidator : AbstractValidator<PlayGameCommand>
{
    public PlayGameCommandValidator()
    {
        RuleFor(x => x.PlayerChoiceId)
            .InclusiveBetween(1, 5); // based on seeded choices
    }
}
