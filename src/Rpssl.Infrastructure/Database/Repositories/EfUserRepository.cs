using Microsoft.EntityFrameworkCore;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Repositories;

namespace Rpssl.Infrastructure.Database.Repositories;

public class EfUserRepository(RpsslDbContext db) : IUserRepository
{
    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        => db.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await db.Users.AddAsync(user, ct);
}
