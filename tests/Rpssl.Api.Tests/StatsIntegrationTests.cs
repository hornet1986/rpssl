using System.Net;
using System.Net.Http.Json;
using Moq;
using Rpssl.Api.Tests.Infrastructure;
using Rpssl.Domain.Enums;

namespace Rpssl.Api.Tests;

[TestClass]
public sealed class StatsIntegrationTests
{
    private static CustomWebApplicationFactory _factory = null!;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        _factory = new CustomWebApplicationFactory();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _factory.Dispose();
    }

    [TestMethod]
    public async Task Get_ReturnsAggregatedStats()
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

        using HttpClient client = _factory.CreateAnonymousClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/v1/stats");

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
}
