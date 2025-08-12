using Microsoft.EntityFrameworkCore;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Repositories;

namespace Rpssl.Infrastructure.Database.Repositories;

public class EfChoiceRepository(RpsslDbContext dbContext) : IChoiceRepository
{
    public async Task<IReadOnlyList<Choice>> GetAllAsync(CancellationToken ct = default)
        => await dbContext.Choices
            .OrderBy(c => c.Id)
            .ToListAsync(ct);

    public Task<Choice?> GetByIdAsync(int id, CancellationToken ct = default)
        => dbContext.Choices.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task AddAsync(Choice choice, CancellationToken ct = default)
    {
        await dbContext.Choices.AddAsync(choice, ct);
    }
}
