using Microsoft.EntityFrameworkCore;
using Rpssl.Domain.Choices;
using Rpssl.Domain.GameOutcome;
using Rpssl.Domain.GameResults;
using Rpssl.Infrastructure.Database;
using Rpssl.Infrastructure.Database.Repositories;

namespace Rpssl.Infrastructure.Tests.Database.Repositories;

[TestClass]
public class EfGameResultRepositoryTests
{
    private DbContextOptions<RpsslDbContext> _dbContextOptions = null!;

    [TestInitialize]
    public void Setup()
    {
        _dbContextOptions = new DbContextOptionsBuilder<RpsslDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [TestMethod]
    public async Task AddAsync_AddsGameResult()
    {
        await using var context = new RpsslDbContext(_dbContextOptions);
        var repository = new EfGameResultRepository(context);
        var result = new GameResult(1, 2, GameOutcome.PlayerWin);
        await repository.AddAsync(result);
        await context.SaveChangesAsync();
        Assert.AreEqual(1, context.GameResults.Count());
        Assert.AreEqual(GameOutcome.PlayerWin, context.GameResults.First().Outcome);
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsCorrectGameResultWithChoices()
    {
        await using var context = new RpsslDbContext(_dbContextOptions);
        var playerChoice = new Choice(1, "Rock");
        var computerChoice = new Choice(2, "Paper");
        context.Choices.AddRange(playerChoice, computerChoice);
        var result = new GameResult(1, 2, GameOutcome.ComputerWin);
        context.GameResults.Add(result);
        await context.SaveChangesAsync();
        var repository = new EfGameResultRepository(context);

        GameResult? fetched = await repository.GetByIdAsync(result.Id);
        Assert.IsNotNull(fetched);
        Assert.AreEqual(GameOutcome.ComputerWin, fetched!.Outcome);
        Assert.IsNotNull(fetched.PlayerChoice);
        Assert.IsNotNull(fetched.ComputerChoice);
        Assert.AreEqual("Rock", fetched.PlayerChoice!.Name);
        Assert.AreEqual("Paper", fetched.ComputerChoice!.Name);
    }

    [TestMethod]
    public async Task GetRecentAsync_ReturnsMostRecentGameResultsWithChoices()
    {
        await using var context = new RpsslDbContext(_dbContextOptions);
        var playerChoice = new Choice(1, "Rock");
        var computerChoice = new Choice(2, "Paper");
        context.Choices.AddRange(playerChoice, computerChoice);
        var result1 = new GameResult(1, 2, GameOutcome.Draw);
        var result2 = new GameResult(1, 2, GameOutcome.PlayerWin);
        context.GameResults.AddRange(result1, result2);
        // Set PlayedAt to control order
        result1.GetType().GetProperty("PlayedAt")?.SetValue(result1, DateTimeOffset.UtcNow.AddMinutes(-10));
        result2.GetType().GetProperty("PlayedAt")?.SetValue(result2, DateTimeOffset.UtcNow);
        await context.SaveChangesAsync();
        var repository = new EfGameResultRepository(context);

        IReadOnlyList<GameResult> recent = await repository.GetRecentAsync(1);
        Assert.AreEqual(1, recent.Count);
        Assert.AreEqual(GameOutcome.PlayerWin, recent[0].Outcome);
        Assert.IsNotNull(recent[0].PlayerChoice);
        Assert.IsNotNull(recent[0].ComputerChoice);
    }

    [TestMethod]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        await using var context = new RpsslDbContext(_dbContextOptions);
        context.GameResults.AddRange(
            new GameResult(1, 2, GameOutcome.Draw),
            new GameResult(1, 2, GameOutcome.PlayerWin)
        );
        await context.SaveChangesAsync();
        var repository = new EfGameResultRepository(context);

        int count = await repository.CountAsync();
        Assert.AreEqual(2, count);
    }

    [TestMethod]
    public async Task CountByOutcomeAsync_ReturnsCorrectCounts()
    {
        await using var context = new RpsslDbContext(_dbContextOptions);
        context.GameResults.AddRange(
            new GameResult(1, 2, GameOutcome.Draw),
            new GameResult(1, 2, GameOutcome.PlayerWin),
            new GameResult(1, 2, GameOutcome.PlayerWin)
        );
        await context.SaveChangesAsync();
        var repository = new EfGameResultRepository(context);

        IDictionary<GameOutcome, int> dict = await repository.CountByOutcomeAsync();
        Assert.AreEqual(2, dict.Count);
        Assert.AreEqual(1, dict[GameOutcome.Draw]);
        Assert.AreEqual(2, dict[GameOutcome.PlayerWin]);
    }

    [TestMethod]
    public async Task DeleteAllAsync_RemovesAllRows()
    {
        await using var context = new RpsslDbContext(_dbContextOptions);
        context.GameResults.AddRange(
            new GameResult(1, 2, GameOutcome.Draw),
            new GameResult(1, 2, GameOutcome.PlayerWin)
        );
        await context.SaveChangesAsync();
        var repository = new EfGameResultRepository(context);

        int deleted = await repository.DeleteAllAsync();

        Assert.AreEqual(2, deleted);
    }
}
