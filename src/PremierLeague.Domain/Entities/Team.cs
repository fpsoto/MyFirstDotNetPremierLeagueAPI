using PremierLeague.Domain.Exceptions;

namespace PremierLeague.Domain.Entities;

public class Team : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string ShortName { get; private set; } = default!;
    public string Stadium { get; private set; } = default!;
    public int StadiumCapacity { get; private set; }
    public string Coach { get; private set; } = default!;
    public int FoundedYear { get; private set; }
    public string City { get; private set; } = default!;
    public string PrimaryColor { get; private set; } = default!;

    public ICollection<Player> Players { get; private set; } = new List<Player>();
    public ICollection<Match> HomeMatches { get; private set; } = new List<Match>();
    public ICollection<Match> AwayMatches { get; private set; } = new List<Match>();
    public ICollection<LeagueStanding> Standings { get; private set; } = new List<LeagueStanding>();

    private Team() { }

    public static Team Create(
        string name, string shortName, string stadium, int stadiumCapacity,
        string coach, int foundedYear, string city, string primaryColor)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Team name cannot be empty.");
        if (foundedYear < 1800 || foundedYear > DateTime.UtcNow.Year)
            throw new DomainException($"Founded year {foundedYear} is not valid.");

        return new Team
        {
            Name = name,
            ShortName = shortName,
            Stadium = stadium,
            StadiumCapacity = stadiumCapacity,
            Coach = coach,
            FoundedYear = foundedYear,
            City = city,
            PrimaryColor = primaryColor
        };
    }

    public void UpdateCoach(string coach)
    {
        if (string.IsNullOrWhiteSpace(coach))
            throw new DomainException("Coach name cannot be empty.");
        Coach = coach;
        SetUpdated();
    }
}
