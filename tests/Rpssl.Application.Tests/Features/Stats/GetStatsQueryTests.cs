using Moq;
using Rpssl.Application.Features.Stats;
using Rpssl.Application.Features.Stats.Queries;
using Rpssl.Domain.Enums;
using Rpssl.Domain.Repositories;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Tests.Features.Stats;

[TestClass]
public class GetStatsQueryTests
{
    [TestMethod]
    public async Task Handle_ReturnsComputedDto()
    {
        // Arrange
        var repo = new Mock<IGameResultRepository>();
        repo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);
        repo.Setup(r => r.CountByOutcomeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<GameOutcome, int>
            {
                [GameOutcome.PlayerWin] = 4,
                [GameOutcome.ComputerWin] = 3,
                [GameOutcome.Draw] = 3
            });

        var handler = new GetStatsQueryHandler(repo.Object);

        // Act
        Result<StatsDto> result = await handler.Handle(new GetStatsQuery(), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(10, result.Value.TotalGames);
        Assert.AreEqual(4, result.Value.PlayerWins);
        Assert.AreEqual(3, result.Value.ComputerWins);
        Assert.AreEqual(3, result.Value.Draws);
    }

    [TestMethod]
    public async Task Handle_ReturnsZeros_WhenNoOutcomes()
    {
        // Arrange
        var repo = new Mock<IGameResultRepository>();
        repo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        repo.Setup(r => r.CountByOutcomeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<GameOutcome, int>());

        var handler = new GetStatsQueryHandler(repo.Object);

        // Act
        Result<StatsDto> result = await handler.Handle(new GetStatsQuery(), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(0, result.Value.TotalGames);
        Assert.AreEqual(0, result.Value.PlayerWins);
        Assert.AreEqual(0, result.Value.ComputerWins);
        Assert.AreEqual(0, result.Value.Draws);
    }
}
