using Rpssl.Api.Tests.Infrastructure;

namespace Rpssl.Api.Tests;

[TestClass]
public sealed class HealthCheckTests
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
    public async Task HealthCheck_ReturnsOk()
    {
        // Arrange
        using HttpClient client = _factory.CreateAnonymousClient();

        // Act
        HttpResponseMessage response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
