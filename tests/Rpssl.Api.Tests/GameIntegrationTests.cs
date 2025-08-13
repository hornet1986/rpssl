using System.Net;
using System.Net.Http.Json;
using Moq;
using Rpssl.Api.Tests.Infrastructure;
using Rpssl.Domain.Entities;
using Rpssl.Domain.Enums;
using Rpssl.SharedKernel;

namespace Rpssl.Api.Tests;

[TestClass]
public sealed class GameIntegrationTests
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
    public async Task Play_ReturnsOk_AndPersistsResult()
    {
        // Arrange
        Choice rock = new(1, "Rock");
        _factory.ChoiceRepositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rock);

        _factory.RandomNumberServiceMock
            .Setup(s => s.GetRandomOneToFiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(2)); // Computer picks Paper

        GameResult? saved = null;
        _factory.GameResultRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()))
            .Callback<GameResult, CancellationToken>((gr, _) => saved = gr)
            .Returns(Task.CompletedTask);

        _factory.UnitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        using HttpClient client = _factory.CreateAnonymousClient();

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/v1/game/play", new { playerChoiceId = 1 });

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        _factory.GameResultRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()), Times.Once);
        _factory.UnitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsNotNull(saved);
        Assert.AreEqual(1, saved!.PlayerChoiceId);
        Assert.AreEqual(2, saved.ComputerChoiceId);
        Assert.AreEqual(GameOutcome.ComputerWin, saved.Outcome);
    }
}
