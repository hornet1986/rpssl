using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Rpssl.Application.Abstractions;
using Rpssl.Domain.Choices;
using Rpssl.Domain.GameResults;
using Rpssl.Domain.UnitOfWork;
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
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(defaultScheme: "TestScheme")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                        "TestScheme", options => { });
            });

            var settings = new Dictionary<string, string?>
            {
                ["RandomNumber:BaseUrl"] = "http://localhost/",
                ["Jwt:Issuer"] = "Rpssl.Tests",
                ["Jwt:Audience"] = "Rpssl.Tests.Client",
                ["Jwt:SigningKey"] = "LOCAL_DEV_ONLY_CHANGE_ME_32chars_min_LENGTH_1234",
                ["Jwt:AccessTokenMinutes"] = "30",
                ["Jwt:RefreshTokenDays"] = "7"
            };
            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices((_, services) =>
        {
            // Remove the existing database context
            RemoveService<DbContextOptions<RpsslDbContext>>(services);
            services.AddDbContext<RpsslDbContext>(options => options.UseInMemoryDatabase($"RpsslApiTests_{Guid.NewGuid()}"));
            // Remove real infrastructure registrations that we will mock (DbContext/repositories/unitofwork, http clients)
            RemoveService<IChoiceRepository>(services);
            RemoveService<IGameResultRepository>(services);
            RemoveService<IRandomNumberService>(services);

            // Add our mocks
            services.AddSingleton(ChoiceRepositoryMock.Object);
            services.AddSingleton(GameResultRepositoryMock.Object);
            services.AddSingleton(RandomNumberServiceMock.Object);

            // (Keep real authentication for integration tests using real JWT)
            services.AddAuthentication(); // ensure auth services present

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
        AllowAutoRedirect = false,
    });
}
