namespace PremierLeague.Domain.Entities;

public class LeagueStanding : BaseEntity
{
    public Guid TeamId { get; private set; }
    public Guid SeasonId { get; private set; }
    public int Position { get; private set; }
    public int Played { get; private set; }
    public int Won { get; private set; }
    public int Drawn { get; private set; }
    public int Lost { get; private set; }
    public int GoalsFor { get; private set; }
    public int GoalsAgainst { get; private set; }
    public int Points { get; private set; }

    public int GoalDifference => GoalsFor - GoalsAgainst;

    public Team Team { get; private set; } = default!;
    public Season Season { get; private set; } = default!;

    private LeagueStanding() { }

    public static LeagueStanding Create(Guid teamId, Guid seasonId)
        => new() { TeamId = teamId, SeasonId = seasonId };

    public void Update(int position, int played, int won, int drawn, int lost, int goalsFor, int goalsAgainst)
    {
        Position = position;
        Played = played;
        Won = won;
        Drawn = drawn;
        Lost = lost;
        GoalsFor = goalsFor;
        GoalsAgainst = goalsAgainst;
        Points = (won * 3) + drawn;
        SetUpdated();
    }
}
