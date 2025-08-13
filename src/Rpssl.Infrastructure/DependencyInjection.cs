using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rpssl.Application.Abstractions;
using Rpssl.Domain.Exceptions;
using Rpssl.Domain.Repositories;
using Rpssl.Infrastructure.Auth;
using Rpssl.Infrastructure.Database;
using Rpssl.Infrastructure.Database.Repositories;
using Rpssl.Infrastructure.Database.UnitOfWork;
using Rpssl.Infrastructure.Random;
using Rpssl.Infrastructure.Time;

namespace Rpssl.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    => services
        .AddDatabase(configuration)
        .AddRepositories()
        .AddUnitOfWork()
        .AddAuth(configuration)
        .AddExternalServices(configuration)
        .AddHealthChecks(configuration);

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = GetConnectionString(configuration);
        services.AddDbContext<RpsslDbContext>(options =>
        {
            SqlConnection sqlConnection = GetSqlConnection(connectionString);
            options.UseSqlServer(sqlConnection);
        });
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IChoiceRepository, EfChoiceRepository>();
        services.AddScoped<IGameResultRepository, EfGameResultRepository>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IRefreshTokenRepository, EfRefreshTokenRepository>();
        return services;
    }

    private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        return services;
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RandomNumberOptions>(configuration.GetSection(RandomNumberOptions.SectionName));

        services.AddHttpClient<IRandomNumberService, RandomNumberService>((sp, http) =>
        {
            IOptions<RandomNumberOptions> optsAccessor = sp.GetRequiredService<IOptions<RandomNumberOptions>>();
            RandomNumberOptions opts = optsAccessor.Value;
            if (!string.IsNullOrWhiteSpace(opts.BaseUrl))
            {
                http.BaseAddress = new Uri(opts.BaseUrl);
            }
        });

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddSqlServer(
                GetConnectionString(configuration),
                name: "RpsslDatabase",
                failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                tags: ["db", "sql"]);
        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<ITimeProvider, SystemTimeProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        JwtOptions jwt = new();
        configuration.GetSection(JwtOptions.SectionName).Bind(jwt);
        byte[] keyBytes = Encoding.UTF8.GetBytes(jwt.SigningKey);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false; // Allow HTTP for local development & tests
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();
        return services;
    }

    private static string GetConnectionString(IConfiguration config)
    {
        const string connectionStringsConfig = "ConnectionStrings:RpsslDatabase";

        return config[connectionStringsConfig] ?? throw new ConfigurationLoadException(connectionStringsConfig);
    }

    private static SqlConnection GetSqlConnection(string connectionString) => new(connectionString);
}
