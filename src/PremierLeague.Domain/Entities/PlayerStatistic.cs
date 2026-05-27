namespace PremierLeague.Domain.Entities;

public class PlayerStatistic : BaseEntity
{
    public Guid PlayerId { get; private set; }
    public Guid SeasonId { get; private set; }
    public int Goals { get; private set; }
    public int Assists { get; private set; }
    public int Appearances { get; private set; }
    public int YellowCards { get; private set; }
    public int RedCards { get; private set; }
    public int MinutesPlayed { get; private set; }
    public int CleanSheets { get; private set; }
    public int GoalsConceded { get; private set; }

    public Player Player { get; private set; } = default!;
    public Season Season { get; private set; } = default!;

    public double GoalsPerGame => Appearances > 0 ? Math.Round((double)Goals / Appearances, 2) : 0;
    public double AssistsPerGame => Appearances > 0 ? Math.Round((double)Assists / Appearances, 2) : 0;

    private PlayerStatistic() { }

    public static PlayerStatistic Create(Guid playerId, Guid seasonId)
        => new() { PlayerId = playerId, SeasonId = seasonId };

    public void Update(int goals, int assists, int appearances, int yellowCards, int redCards,
        int minutesPlayed, int cleanSheets = 0, int goalsConceded = 0)
    {
        Goals = goals;
        Assists = assists;
        Appearances = appearances;
        YellowCards = yellowCards;
        RedCards = redCards;
        MinutesPlayed = minutesPlayed;
        CleanSheets = cleanSheets;
        GoalsConceded = goalsConceded;
        SetUpdated();
    }
}
