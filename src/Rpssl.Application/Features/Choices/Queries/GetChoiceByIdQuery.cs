using Rpssl.Application.Abstractions;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Choices.Queries;

public sealed record GetChoiceByIdQuery(int Id) : IRequest<Result<ChoiceDto>>;
