using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Rpssl.Api.Tests.Infrastructure;
using Rpssl.Domain.GameOutcome;

namespace Rpssl.Api.Tests;

[TestClass]
public sealed class GameResultsIntegrationTests
{
    private static CustomWebApplicationFactory _factory = null!;
    private static HttpClient _client = null!;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(defaultScheme: "TestScheme")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "TestScheme", options => { });
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: "TestScheme");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _factory.Dispose();
        _client.Dispose();
    }

    [TestMethod]
    public async Task Get_ReturnsAggregatedGameResults()
    {
        // Arrange
        _factory.GameResultRepositoryMock
            .Setup(r => r.CountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        _factory.GameResultRepositoryMock
            .Setup(r => r.CountByOutcomeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<GameOutcome, int>
            {
                [GameOutcome.PlayerWin] = 4,
                [GameOutcome.ComputerWin] = 3,
                [GameOutcome.Draw] = 3
            });

        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/v1.0/GameResults");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        StatsDto? payload = await response.Content.ReadFromJsonAsync<StatsDto>();
        Assert.IsNotNull(payload);
        Assert.AreEqual(10, payload.TotalGames);
        Assert.AreEqual(4, payload.PlayerWins);
        Assert.AreEqual(3, payload.ComputerWins);
        Assert.AreEqual(3, payload.Draws);
    }

    private sealed record StatsDto(int TotalGames, int PlayerWins, int ComputerWins, int Draws);

    [TestMethod]
    public async Task Get_Unauthorized_WithoutToken()
    {
        using HttpClient client = _factory.CreateAnonymousClient();
        HttpResponseMessage response = await client.GetAsync("/api/v1.0/GameResults");
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task ClearResults_ReturnsOk_AndDeletesAll()
    {
        _factory.GameResultRepositoryMock
            .Setup(r => r.DeleteAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        HttpResponseMessage response = await _client.DeleteAsync("api/v1.0/GameResults");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        _factory.GameResultRepositoryMock.Verify(r => r.DeleteAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
