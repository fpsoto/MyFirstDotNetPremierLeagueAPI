using PremierLeague.Application.Features.Teams.Queries.GetTeams;
using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.UnitTests.Features.Teams;

public class GetTeamsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly GetTeamsQueryHandler _handler;

    public GetTeamsQueryHandlerTests()
    {
        _handler = new GetTeamsQueryHandler(_uowMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPagedTeams_WhenTeamsExist()
    {
        var teams = new List<Team>
        {
            Team.Create("Arsenal", "ARS", "Emirates", 60000, "Coach A", 1886, "London", "Red"),
            Team.Create("Liverpool", "LIV", "Anfield", 55000, "Coach B", 1892, "Liverpool", "Red"),
            Team.Create("Chelsea", "CHE", "Stamford Bridge", 40000, "Coach C", 1905, "London", "Blue")
        };

        _uowMock.Setup(u => u.Teams.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        var query = new GetTeamsQuery(null, null, null, false, 1, 10);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task Handle_FiltersBySearch_ReturnsMatchingTeams()
    {
        var teams = new List<Team>
        {
            Team.Create("Arsenal", "ARS", "Emirates", 60000, "Coach", 1886, "London", "Red"),
            Team.Create("Manchester City", "MCI", "Etihad", 55000, "Coach", 1880, "Manchester", "Blue")
        };

        _uowMock.Setup(u => u.Teams.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        var query = new GetTeamsQuery("Arsenal", null, null, false, 1, 10);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Arsenal");
    }

    [Fact]
    public async Task Handle_FiltersByCity_ReturnsOnlyLondonTeams()
    {
        var teams = new List<Team>
        {
            Team.Create("Arsenal", "ARS", "Emirates", 60000, "Coach", 1886, "London", "Red"),
            Team.Create("Chelsea", "CHE", "Stamford", 40000, "Coach", 1905, "London", "Blue"),
            Team.Create("Manchester City", "MCI", "Etihad", 55000, "Coach", 1880, "Manchester", "Blue")
        };

        _uowMock.Setup(u => u.Teams.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        var query = new GetTeamsQuery(null, "London", null, false, 1, 10);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(t => t.City.Should().Be("London"));
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsCorrectPage()
    {
        var teams = Enumerable.Range(1, 25)
            .Select(i => Team.Create($"Team {i:D2}", $"T{i:D2}", "Stadium", 40000, "Coach", 1900, "City", "Red"))
            .ToList();

        _uowMock.Setup(u => u.Teams.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        var query = new GetTeamsQuery(null, null, null, false, 2, 10);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(10);
        result.TotalCount.Should().Be(25);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }
}
