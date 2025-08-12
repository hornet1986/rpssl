using Rpssl.Domain.Entities;

namespace Rpssl.Application.Features.Choices.Mappings;

public static class ChoiceMappings
{
    public static ChoiceDto ToDto(this Choice entity) => new(entity.Id, entity.Name);
}
