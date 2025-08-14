using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Choices.Queries;

public record GetChoicesQuery() : IRequest<Result<IReadOnlyList<ChoiceDto>>>;
