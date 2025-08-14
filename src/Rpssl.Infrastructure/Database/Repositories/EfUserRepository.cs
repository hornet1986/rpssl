using Microsoft.EntityFrameworkCore;
using Rpssl.Domain.Users;

namespace Rpssl.Infrastructure.Database.Repositories;

public class EfUserRepository(RpsslDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => dbContext.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await dbContext.Users.AddAsync(user, ct);
}
