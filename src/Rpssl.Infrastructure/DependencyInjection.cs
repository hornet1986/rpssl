using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rpssl.Domain.Exceptions;
using Rpssl.Domain.Repositories;
using Rpssl.Infrastructure.Database;
using Rpssl.Infrastructure.Database.Repositories;
using Rpssl.Infrastructure.Database.UnitOfWork;

namespace Rpssl.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = GetConnectionString(configuration);

        services.AddDbContext<RpsslDbContext>(options =>
        {
            SqlConnection sqlConnection = GetSqlConnection(connectionString);
            options.UseSqlServer(sqlConnection);
        });

        // Repositories
        services.AddScoped<IChoiceRepository, EfChoiceRepository>();
        services.AddScoped<IGameResultRepository, EfGameResultRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return services;
    }

    private static string GetConnectionString(IConfiguration config)
    {
        const string connectionStringsConfig = "ConnectionStrings:RpsslDatabase";

        return config[connectionStringsConfig] ?? throw new ConfigurationLoadException(connectionStringsConfig);
    }

    private static SqlConnection GetSqlConnection(string connectionString) => new(connectionString);
}
