using Moq;
using Rpssl.Application.Choices;
using Rpssl.Application.Choices.Queries;
using Rpssl.Domain.Choices;
using Rpssl.SharedKernel;

namespace Rpssl.Application.Tests.Choices;

[TestClass]
public class GetChoiceByIdQueryHandlerTests
{
    [TestMethod]
    public async Task Handle_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        var repo = new Mock<IChoiceRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Choice?)null);

        var handler = new GetChoiceByIdQueryHandler(repo.Object);

        // Act
        Result<ChoiceDto> result = await handler.Handle(new GetChoiceByIdQuery(42), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Choices.NotFound", result.Error.Code);
    }

    [TestMethod]
    public async Task Handle_ReturnsDto_WhenFound()
    {
        // Arrange
        var repo = new Mock<IChoiceRepository>();
        repo.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Choice(2, "Paper"));

        var handler = new GetChoiceByIdQueryHandler(repo.Object);

        // Act
        Result<ChoiceDto> result = await handler.Handle(new GetChoiceByIdQuery(2), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Paper", result.Value.Name);
    }
}
