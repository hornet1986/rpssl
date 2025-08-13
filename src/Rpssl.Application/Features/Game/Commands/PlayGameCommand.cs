using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Game.Commands;

public sealed record PlayGameCommand(int PlayerChoiceId) : IRequest<Result<GameResultDto>>;
