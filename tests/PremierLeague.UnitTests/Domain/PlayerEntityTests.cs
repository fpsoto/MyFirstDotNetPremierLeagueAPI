using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Exceptions;

namespace PremierLeague.UnitTests.Domain;

public class PlayerEntityTests
{
    private static readonly Guid TeamId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ShouldReturnPlayer()
    {
        var player = Player.Create("Bukayo", "Saka", 22, "English", PlayerPosition.Forward, 7, 100m, TeamId);

        player.FullName.Should().Be("Bukayo Saka");
        player.Position.Should().Be(PlayerPosition.Forward);
        player.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(15)]
    [InlineData(46)]
    public void Create_WithInvalidAge_ShouldThrowDomainException(int age)
    {
        var act = () => Player.Create("John", "Doe", age, "English", PlayerPosition.Forward, 9, 50m, TeamId);
        act.Should().Throw<DomainException>().WithMessage("*age*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Create_WithInvalidShirtNumber_ShouldThrowDomainException(int number)
    {
        var act = () => Player.Create("John", "Doe", 25, "English", PlayerPosition.Midfielder, number, 50m, TeamId);
        act.Should().Throw<DomainException>().WithMessage("*shirt*");
    }

    [Fact]
    public void Transfer_ToNewTeam_ShouldUpdateTeamId()
    {
        var player = Player.Create("John", "Doe", 25, "English", PlayerPosition.Midfielder, 10, 50m, TeamId);
        var newTeamId = Guid.NewGuid();

        player.Transfer(newTeamId);

        player.TeamId.Should().Be(newTeamId);
        player.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Retire_ShouldDeactivatePlayer()
    {
        var player = Player.Create("John", "Doe", 35, "English", PlayerPosition.Goalkeeper, 1, 5m, TeamId);
        player.Retire();

        player.IsActive.Should().BeFalse();
    }
}
