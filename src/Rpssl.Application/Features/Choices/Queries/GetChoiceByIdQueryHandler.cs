using MediatR;
using Rpssl.Application.Features.Choices.Mappings;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Repositories;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Choices.Queries;

internal sealed class GetChoiceByIdQueryHandler(IChoiceRepository repo) : IRequestHandler<GetChoiceByIdQuery, Result<ChoiceDto>>
{
    private static readonly Error ChoiceNotFound = Error.NotFound("Choices.NotFound", "Choice not found");

    public async Task<Result<ChoiceDto>> Handle(GetChoiceByIdQuery request, CancellationToken cancellationToken)
    {
        Choice? entity = await repo.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? Result.Failure<ChoiceDto>(ChoiceNotFound) : Result.Success(entity.ToDto());
    }
}
