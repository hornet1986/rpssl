using Moq;
using Rpssl.Application.Choices;
using Rpssl.Application.Choices.Queries;
using Rpssl.Domain.Choices;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Tests.Features.Choices;

[TestClass]
public class GetChoicesQueryHandlerTests
{
    [TestMethod]
    public async Task Handle_ReturnsDtos_WhenRepoHasItems()
    {
        // Arrange
        var items = new List<Choice> { new(1, "Rock"), new(2, "Paper") };
        var repo = new Mock<IChoiceRepository>();
        repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var handler = new GetChoicesQueryHandler(repo.Object);

        // Act
        Result<IReadOnlyList<ChoiceDto>> result = await handler.Handle(new GetChoicesQuery(), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2, result.Value.Count);
        Assert.AreEqual("Rock", result.Value[0].Name);
        Assert.AreEqual(2, result.Value[1].Id);
    }

    [TestMethod]
    public async Task Handle_ReturnsEmptyList_WhenRepoEmpty()
    {
        // Arrange
        var repo = new Mock<IChoiceRepository>();
        repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<Choice>) []);

        var handler = new GetChoicesQueryHandler(repo.Object);

        // Act
        Result<IReadOnlyList<ChoiceDto>> result = await handler.Handle(new GetChoicesQuery(), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(0, result.Value.Count);
    }
}
