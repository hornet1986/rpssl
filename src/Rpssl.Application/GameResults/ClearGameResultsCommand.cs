using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Application.GameResults;

public sealed record ClearGameResultsCommand : IRequest<Result>;
