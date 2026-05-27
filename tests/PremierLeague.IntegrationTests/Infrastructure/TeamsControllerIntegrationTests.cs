using PremierLeague.IntegrationTests.Infrastructure;

namespace PremierLeague.IntegrationTests;

public class TeamsControllerIntegrationTests(PremierLeagueWebApplicationFactory factory)
    : IClassFixture<PremierLeagueWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetTeams_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/teams");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTeams_ReturnsPaginatedResult()
    {
        var response = await _client.GetAsync("/api/teams?pageNumber=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("items");
        content.Should().Contain("totalCount");
    }

    [Fact]
    public async Task GetTeamById_WithInvalidId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/teams/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
