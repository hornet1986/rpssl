using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Rpssl.Application.Abstractions;
using Rpssl.Domain.Repositories;
using Rpssl.Infrastructure.Database;

namespace Rpssl.Api.Tests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IChoiceRepository> ChoiceRepositoryMock { get; } = new();
    public Mock<IGameResultRepository> GameResultRepositoryMock { get; } = new();
    public Mock<IUnitOfWork> UnitOfWorkMock { get; } = new();
    public Mock<IRandomNumberService> RandomNumberServiceMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["RandomNumber:BaseUrl"] = "http://localhost/"
            };
            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices((_, services) =>
        {
            // Remove the existing database context
            RemoveService<DbContextOptions<RpsslDbContext>>(services);
            services.AddDbContext<RpsslDbContext>(options =>
            {
                options.UseInMemoryDatabase($"RpsslApiTests_{Guid.NewGuid()}");
            });
            // Remove real infrastructure registrations that we will mock (DbContext/repositories/unitofwork, http clients)
            RemoveService<IChoiceRepository>(services);
            RemoveService<IGameResultRepository>(services);
            RemoveService<IUnitOfWork>(services);
            RemoveService<IRandomNumberService>(services);

            // Add our mocks
            services.AddSingleton(ChoiceRepositoryMock.Object);
            services.AddSingleton(GameResultRepositoryMock.Object);
            services.AddSingleton(UnitOfWorkMock.Object);
            services.AddSingleton(RandomNumberServiceMock.Object);
        });

        builder.UseEnvironment("Development");
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        ServiceDescriptor? descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }

    public HttpClient CreateAnonymousClient() => CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });
}
