using Rpssl.Application.Abstractions;
using Rpssl.Application.GameResults;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Games;

public sealed record PlayGameCommand(int PlayerChoiceId) : IRequest<Result<GameResultDto>>;
