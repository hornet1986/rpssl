using Rpssl.Application.Abstractions;
using Rpssl.Application.Choices.Mappings;
using Rpssl.Domain.Choices;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Choices.Queries;

internal sealed class GetChoiceByIdQueryHandler(IChoiceRepository repo) : IRequestHandler<GetChoiceByIdQuery, Result<ChoiceDto>>
{
    private static readonly Error ChoiceNotFound = Error.NotFound("Choices.NotFound", "Choice not found");

    public async Task<Result<ChoiceDto>> Handle(GetChoiceByIdQuery request, CancellationToken cancellationToken)
    {
        Choice? entity = await repo.GetByIdAsync(request.Id, cancellationToken);
        return entity?.ToDto() ?? Result.Failure<ChoiceDto>(ChoiceNotFound);
    }
}
