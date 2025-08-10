using Microsoft.EntityFrameworkCore;
using Rpssl.Domain.Entities;
using Rpssl.Infrastructure.Database;
using Rpssl.Infrastructure.Database.Repositories;

namespace Rpssl.Infrastructure.Tests.Database.Repositories;

[TestClass]
public class EfChoiceRepositoryTests
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
    public async Task GetAllAsync_ReturnsOrderedChoices()
    {
        await using var context = new RpsslDbContext(_dbContextOptions);
        context.Choices.AddRange(new Choice(2, "Paper"), new Choice(1, "Rock"));
        await context.SaveChangesAsync();
        var repository = new EfChoiceRepository(context);

        IReadOnlyList<Choice> result = await repository.GetAllAsync();
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(1, result[0].Id);
        Assert.AreEqual(2, result[1].Id);
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsCorrectChoice()
    {
        await using var context = new RpsslDbContext(_dbContextOptions);
        context.Choices.AddRange(new Choice(1, "Rock"), new Choice(2, "Paper"));
        await context.SaveChangesAsync();
        var repository = new EfChoiceRepository(context);

        Choice? result = await repository.GetByIdAsync(2);
        Assert.IsNotNull(result);
        Assert.AreEqual("Paper", result!.Name);
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        await using var context = new RpsslDbContext(_dbContextOptions);
        var repository = new EfChoiceRepository(context);

        Choice? result = await repository.GetByIdAsync(99);
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task AddAsync_AddsChoiceToDbSet()
    {
        await using var context = new RpsslDbContext(_dbContextOptions);
        var repository = new EfChoiceRepository(context);
        var choice = new Choice(3, "Scissors");
        await repository.AddAsync(choice);
        await context.SaveChangesAsync();
        Assert.AreEqual(1, context.Choices.Count());
        Assert.AreEqual("Scissors", context.Choices.First().Name);
    }
}
