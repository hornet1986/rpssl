using Rpssl.Application.Abstractions;
using Rpssl.Application.Choices.Mappings;
using Rpssl.Domain.Choices;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Choices.Queries;

internal sealed class GetChoicesQueryHandler(IChoiceRepository repo) : IRequestHandler<GetChoicesQuery, Result<IReadOnlyList<ChoiceDto>>>
{
    public async Task<Result<IReadOnlyList<ChoiceDto>>> Handle(GetChoicesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Choice> list = await repo.GetAllAsync(cancellationToken);
        return list.Select(c => c.ToDto()).ToList();
    }
}
