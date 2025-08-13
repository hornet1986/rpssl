using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Choices.Queries;

public record GetChoicesQuery() : IRequest<Result<IReadOnlyList<ChoiceDto>>>;
