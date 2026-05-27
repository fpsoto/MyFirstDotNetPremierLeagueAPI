using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Exceptions;

namespace PremierLeague.UnitTests.Domain;

public class TeamEntityTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateTeam()
    {
        var team = Team.Create("Arsenal", "ARS", "Emirates Stadium", 60704, "Mikel Arteta", 1886, "London", "Red");

        team.Name.Should().Be("Arsenal");
        team.ShortName.Should().Be("ARS");
        team.FoundedYear.Should().Be(1886);
        team.Id.Should().NotBeEmpty();
        team.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldThrowDomainException(string? name)
    {
        var act = () => Team.Create(name!, "ARS", "Stadium", 50000, "Coach", 1990, "City", "Red");
        act.Should().Throw<DomainException>().WithMessage("*name*");
    }

    [Theory]
    [InlineData(1799)]
    [InlineData(2100)]
    public void Create_WithInvalidFoundedYear_ShouldThrowDomainException(int year)
    {
        var act = () => Team.Create("Team", "TM", "Stadium", 50000, "Coach", year, "City", "Red");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdateCoach_WithValidName_ShouldUpdateAndSetTimestamp()
    {
        var team = Team.Create("Arsenal", "ARS", "Emirates Stadium", 60704, "Old Coach", 1886, "London", "Red");

        team.UpdateCoach("New Coach");

        team.Coach.Should().Be("New Coach");
        team.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateCoach_WithEmptyName_ShouldThrowDomainException()
    {
        var team = Team.Create("Arsenal", "ARS", "Emirates Stadium", 60704, "Coach", 1886, "London", "Red");

        var act = () => team.UpdateCoach(string.Empty);
        act.Should().Throw<DomainException>();
    }
}
