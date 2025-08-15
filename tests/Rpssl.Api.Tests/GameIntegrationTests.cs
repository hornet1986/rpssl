using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Rpssl.Api.Tests.Infrastructure;
using Rpssl.Domain.Choices;
using Rpssl.Domain.GameOutcome;
using Rpssl.Domain.GameResults;
using Rpssl.SharedKernel;

namespace Rpssl.Api.Tests;

[TestClass]
public sealed class GameIntegrationTests
{
    private static CustomWebApplicationFactory _factory = null!;
    private static HttpClient _client = null!;

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(defaultScheme: "TestScheme")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "TestScheme", options => { });
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: "TestScheme");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _factory.Dispose();
        _client.Dispose();
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

        // Act
        HttpResponseMessage response = await _client.PostAsJsonAsync("api/v1.0/GamePlay", new { playerChoiceId = 1 });

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        _factory.GameResultRepositoryMock.Verify(r => r.AddAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.IsNotNull(saved);
        Assert.AreEqual(1, saved!.PlayerChoiceId);
        Assert.AreEqual(2, saved.ComputerChoiceId);
        Assert.AreEqual(GameOutcome.ComputerWin, saved.Outcome);
    }
}
