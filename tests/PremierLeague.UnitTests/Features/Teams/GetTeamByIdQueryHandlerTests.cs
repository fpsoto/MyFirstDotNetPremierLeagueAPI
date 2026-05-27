using PremierLeague.Application.Features.Teams.Queries.GetTeamById;
using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.UnitTests.Features.Teams;

public class GetTeamByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly GetTeamByIdQueryHandler _handler;
    private static readonly Guid SeasonId = Guid.NewGuid();

    public GetTeamByIdQueryHandlerTests()
    {
        _handler = new GetTeamByIdQueryHandler(_uowMock.Object);
    }

    [Fact]
    public async Task Handle_TeamExists_ReturnsSuccessResult()
    {
        var teamId = Guid.NewGuid();
        var team = Team.Create("Arsenal", "ARS", "Emirates Stadium", 60704, "Arteta", 1886, "London", "Red");

        _uowMock.Setup(u => u.Teams.GetByIdWithPlayersAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);
        // Handler looks up standing by request.TeamId (not team.Id)
        _uowMock.Setup(u => u.Standings.GetByTeamAndSeasonAsync(teamId, SeasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LeagueStanding?)null);

        var result = await _handler.Handle(new GetTeamByIdQuery(teamId, SeasonId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Arsenal");
        result.Value.Stadium.Should().Be("Emirates Stadium");
    }

    [Fact]
    public async Task Handle_TeamNotFound_ReturnsNotFoundResult()
    {
        var teamId = Guid.NewGuid();

        _uowMock.Setup(u => u.Teams.GetByIdWithPlayersAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team?)null);

        var result = await _handler.Handle(new GetTeamByIdQuery(teamId, SeasonId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain(teamId.ToString());
    }

    [Fact]
    public async Task Handle_TeamWithStanding_IncludesStandingInResponse()
    {
        var teamId = Guid.NewGuid();
        var team = Team.Create("Liverpool", "LIV", "Anfield", 61276, "Slot", 1892, "Liverpool", "Red");

        var standing = LeagueStanding.Create(teamId, SeasonId);
        standing.Update(1, 30, 22, 5, 3, 72, 28);

        _uowMock.Setup(u => u.Teams.GetByIdWithPlayersAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(team);
        // Must match request.TeamId, which is teamId — not the entity's auto-generated team.Id
        _uowMock.Setup(u => u.Standings.GetByTeamAndSeasonAsync(teamId, SeasonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(standing);

        var result = await _handler.Handle(new GetTeamByIdQuery(teamId, SeasonId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.CurrentStanding.Should().NotBeNull();
        result.Value.CurrentStanding!.Points.Should().Be(71); // 22*3 + 5
        result.Value.CurrentStanding.Position.Should().Be(1);
    }
}
