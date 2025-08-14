using Moq;
using Rpssl.Application.Abstractions;
using Rpssl.Application.Games;
using Rpssl.Domain.Choices;
using Rpssl.Domain.GameOutcome;
using Rpssl.Domain.GameResults;
using Rpssl.Domain.UnitOfWork;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Tests.Features.Game;

[TestClass]
public class PlayGameCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_ReturnsFailure_WhenPlayerChoiceDoesNotExist()
    {
        // Arrange
        var choices = new Mock<IChoiceRepository>();
        choices.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
               .ReturnsAsync((Choice?)null);

        var results = new Mock<IGameResultRepository>();
        var uow = new Mock<IUnitOfWork>();
        var rules = new Mock<IGameRulesEngine>();
        var rng = new Mock<IRandomNumberService>();
        rng.Setup(s => s.GetRandomOneToFiveAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(Result.Success(1));

        var handler = new PlayGameCommandHandler(choices.Object, results.Object, uow.Object, rules.Object, rng.Object);

        // Act
        Result<GameResultDto> result = await handler.Handle(new PlayGameCommand(1), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Choices.NotFound", result.Error.Code);
        results.Verify(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()), Times.Never);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_ReturnsFailure_WhenRandomServiceFails()
    {
        // Arrange
        var choices = new Mock<IChoiceRepository>();
        choices.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Choice(1, "Rock"));

        var results = new Mock<IGameResultRepository>();
        var uow = new Mock<IUnitOfWork>();
        var rules = new Mock<IGameRulesEngine>();
        var rng = new Mock<IRandomNumberService>();
        rng.Setup(s => s.GetRandomOneToFiveAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(Result.Failure<int>(Error.Problem("Rng.Down", "rng service down")));

        var handler = new PlayGameCommandHandler(choices.Object, results.Object, uow.Object, rules.Object, rng.Object);

        // Act
        Result<GameResultDto> result = await handler.Handle(new PlayGameCommand(1), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Rng.Down", result.Error.Code);
        results.Verify(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()), Times.Never);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_PersistsResult_AndReturnsDto_Success()
    {
        // Arrange
        var choices = new Mock<IChoiceRepository>();
        choices.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Choice(1, "Rock"));

        var rng = new Mock<IRandomNumberService>();
        rng.Setup(s => s.GetRandomOneToFiveAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(Result.Success(3)); // computer picks Scissors

        var rules = new Mock<IGameRulesEngine>();
        rules.Setup(r => r.DecideOutcome(1, 3)).Returns(GameOutcome.PlayerWin);

        var results = new Mock<IGameResultRepository>();
        GameResult? saved = null;
        results.Setup(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()))
               .Callback<GameResult, CancellationToken>((gr, _) => saved = gr)
               .Returns(Task.CompletedTask);

        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new PlayGameCommandHandler(choices.Object, results.Object, uow.Object, rules.Object, rng.Object);

        // Act
        Result<GameResultDto> result = await handler.Handle(new PlayGameCommand(1), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(1, result.Value.PlayerChoiceId);
        Assert.AreEqual(3, result.Value.ComputerChoiceId);
        Assert.AreEqual(GameOutcome.PlayerWin, result.Value.Outcome);
        Assert.IsNotNull(saved);
        Assert.AreEqual(GameOutcome.PlayerWin, saved!.Outcome);
        results.Verify(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
