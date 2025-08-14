using Rpssl.Domain.Choices;

namespace Rpssl.Application.Choices.Mappings;

public static class ChoiceMappings
{
    public static ChoiceDto ToDto(this Choice entity) => new(entity.Id, entity.Name);
}
