namespace Rpssl.Domain.Choices;

public interface IChoiceRepository
{
    Task<IReadOnlyList<Choice>> GetAllAsync(CancellationToken ct = default);
    Task<Choice?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(Choice choice, CancellationToken ct = default);
}
