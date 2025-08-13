using System.Net;
using System.Net.Http.Json;
using Moq;
using Rpssl.Api.Tests.Infrastructure;
using Rpssl.Domain.Entities;

namespace Rpssl.Api.Tests;

[TestClass]
public sealed class ChoicesIntegrationTests
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
    public async Task GetAll_ReturnsSeededChoices()
    {
        // Arrange
        Choice[] choices =
        [
            new(1, "Rock"),
            new(2, "Paper"),
            new(3, "Scissors"),
            new(4, "Lizard"),
            new(5, "Spock"),
        ];

        _factory.ChoiceRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(choices);

        using HttpClient client = _factory.CreateAnonymousClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/api/v1/choices");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        List<ChoiceDto>? payload = await response.Content.ReadFromJsonAsync<List<ChoiceDto>>();
        Assert.IsNotNull(payload);
        Assert.AreEqual(5, payload.Count);
        Assert.IsTrue(payload.Exists(c => c is { Id: 1, Name: "Rock" }));
    }

    private sealed record ChoiceDto(int Id, string Name);
}
