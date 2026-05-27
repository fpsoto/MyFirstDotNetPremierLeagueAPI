using PremierLeague.Domain.Exceptions;

namespace PremierLeague.Domain.Entities;

public class Season : BaseEntity
{
    public string Name { get; private set; } = default!;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public bool IsActive { get; private set; }

    public ICollection<Match> Matches { get; private set; } = new List<Match>();
    public ICollection<LeagueStanding> Standings { get; private set; } = new List<LeagueStanding>();

    private Season() { }

    public static Season Create(string name, DateOnly startDate, DateOnly endDate, bool isActive = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Season name cannot be empty.");
        if (endDate <= startDate)
            throw new DomainException("Season end date must be after start date.");

        return new Season { Name = name, StartDate = startDate, EndDate = endDate, IsActive = isActive };
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
