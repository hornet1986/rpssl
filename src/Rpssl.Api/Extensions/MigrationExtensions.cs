using Microsoft.EntityFrameworkCore;
using Rpssl.Infrastructure.Database;

namespace Rpssl.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using RpsslDbContext dbContext = 
            scope.ServiceProvider.GetRequiredService<RpsslDbContext>();

        dbContext.Database.Migrate();
    }
}
