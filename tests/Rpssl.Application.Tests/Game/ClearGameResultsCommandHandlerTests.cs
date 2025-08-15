using Moq;
using Rpssl.Application.GameResults;
using Rpssl.Domain.GameResults;
using Rpssl.Domain.UnitOfWork;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Tests.Game;

[TestClass]
public class ClearGameResultsCommandHandlerTests
{
    [TestMethod]
    public async Task Handle_DeletesAllAndReturnsSuccess()
    {
        var repo = new Mock<IGameResultRepository>();
        repo.Setup(r => r.DeleteAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);
        var uow = new Mock<IUnitOfWork>();
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        var handler = new ClearGameResultsCommandHandler(repo.Object, uow.Object);

        Result result = await handler.Handle(new ClearGameResultsCommand(), CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        repo.Verify(r => r.DeleteAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
