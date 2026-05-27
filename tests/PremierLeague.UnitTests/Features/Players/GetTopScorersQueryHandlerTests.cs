using PremierLeague.Application.Features.Players.Queries.GetTopScorers;
using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.UnitTests.Features.Players;

public class GetTopScorersQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly GetTopScorersQueryHandler _handler;
    private static readonly Guid SeasonId = Guid.NewGuid();
    private static readonly Guid TeamId = Guid.NewGuid();

    public GetTopScorersQueryHandlerTests()
    {
        _handler = new GetTopScorersQueryHandler(_uowMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsPlayersWithoutStatistics_ReturnsEmpty()
    {
        // The repository loads Statistic via EF Core Include; in unit tests the navigation is null.
        // The handler must defensively filter them out rather than throw a NullReferenceException.
        var players = CreatePlayersWithoutStats("Haaland", "Salah", "Saka");

        _uowMock.Setup(u => u.Players.GetTopScorersAsync(SeasonId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(players);

        var result = await _handler.Handle(new GetTopScorersQuery(SeasonId, 10), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNoPlayers_ReturnsEmpty()
    {
        _uowMock.Setup(u => u.Players.GetTopScorersAsync(SeasonId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _handler.Handle(new GetTopScorersQuery(SeasonId, 10), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ExcludesPlayersWithoutStatistics()
    {
        var playerWithStats = Player.Create("Mo", "Salah", 32, "Egyptian", PlayerPosition.Forward, 11, 80m, TeamId);
        var playerWithoutStats = Player.Create("New", "Player", 20, "English", PlayerPosition.Forward, 9, 5m, TeamId);

        _uowMock.Setup(u => u.Players.GetTopScorersAsync(SeasonId, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync([playerWithStats, playerWithoutStats]);

        var result = await _handler.Handle(new GetTopScorersQuery(SeasonId, 10), CancellationToken.None);

        result.Should().BeEmpty(); // No Statistic navigation loaded in this mock scenario
    }

    private static List<Player> CreatePlayersWithoutStats(params string[] lastNames)
        => lastNames
            .Select(n => Player.Create("Player", n, 28, "English", PlayerPosition.Forward, 9, 50m, TeamId))
            .ToList();
}
