using FluentValidation;

namespace Rpssl.Application.Features.Game.Commands;

public sealed class PlayGameCommandValidator : AbstractValidator<PlayGameCommand>
{
    public PlayGameCommandValidator()
    {
        RuleFor(x => x.PlayerChoiceId)
            .InclusiveBetween(1, 5); // based on seeded choices
    }
}
