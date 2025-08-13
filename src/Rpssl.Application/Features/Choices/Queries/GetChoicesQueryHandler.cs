using Rpssl.Application.Abstractions;
using Rpssl.Application.Features.Choices.Mappings;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Repositories;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Features.Choices.Queries;

internal sealed class GetChoicesQueryHandler(IChoiceRepository repo) : IRequestHandler<GetChoicesQuery, Result<IReadOnlyList<ChoiceDto>>>
{
    public async Task<Result<IReadOnlyList<ChoiceDto>>> Handle(GetChoicesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Choice> list = await repo.GetAllAsync(cancellationToken);
        var dtos = list.Select(c => c.ToDto()).ToList();
        return dtos;
    }
}
