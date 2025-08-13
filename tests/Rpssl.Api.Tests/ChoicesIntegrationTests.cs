using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Rpssl.Api.Tests.Infrastructure;
using Rpssl.Domain.Entities;

namespace Rpssl.Api.Tests;

[TestClass]
public sealed class ChoicesIntegrationTests
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

        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/v1/choices");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        List<ChoiceDto>? payload = await response.Content.ReadFromJsonAsync<List<ChoiceDto>>();
        Assert.IsNotNull(payload);
        Assert.AreEqual(5, payload.Count);
        Assert.IsTrue(payload.Exists(c => c is { Id: 1, Name: "Rock" }));
    }

    [TestMethod]
    public async Task GetById_ExistingChoice_ReturnsChoice()
    {
        // Arrange
        const int id = 3;
        var entity = new Choice(id, "Scissors");

        _factory.ChoiceRepositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        // Act
        HttpResponseMessage response = await _client.GetAsync($"/api/v1/choices/{id}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        ChoiceDto? payload = await response.Content.ReadFromJsonAsync<ChoiceDto>();
        Assert.IsNotNull(payload);
        Assert.AreEqual(id, payload.Id);
        Assert.AreEqual("Scissors", payload.Name);
    }

    private sealed record ChoiceDto(int Id, string Name);

    [TestMethod]
    public async Task GetAll_Unauthorized_WithoutToken()
    {
        using HttpClient client = _factory.CreateAnonymousClient();
        HttpResponseMessage response = await client.GetAsync("/api/v1/choices");
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
