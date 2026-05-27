using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Exceptions;

namespace PremierLeague.Domain.Entities;

public class Match : BaseEntity
{
    public Guid HomeTeamId { get; private set; }
    public Guid AwayTeamId { get; private set; }
    public Guid SeasonId { get; private set; }
    public int? HomeGoals { get; private set; }
    public int? AwayGoals { get; private set; }
    public DateTime MatchDate { get; private set; }
    public int Matchday { get; private set; }
    public MatchStatus Status { get; private set; }
    public int? AttendanceCount { get; private set; }

    public Team HomeTeam { get; private set; } = default!;
    public Team AwayTeam { get; private set; } = default!;
    public Season Season { get; private set; } = default!;

    // Derived outcome helpers — avoids scattering this logic across handlers
    public bool IsHomeWin => Status == MatchStatus.Completed && HomeGoals > AwayGoals;
    public bool IsAwayWin => Status == MatchStatus.Completed && AwayGoals > HomeGoals;
    public bool IsDraw => Status == MatchStatus.Completed && HomeGoals == AwayGoals;

    private Match() { }

    public static Match Schedule(Guid homeTeamId, Guid awayTeamId, Guid seasonId, DateTime matchDate, int matchday)
    {
        if (homeTeamId == awayTeamId)
            throw new DomainException("A team cannot play against itself.");
        if (matchday < 1 || matchday > 38)
            throw new DomainException($"Matchday {matchday} is outside the valid range (1-38).");

        return new Match
        {
            HomeTeamId = homeTeamId,
            AwayTeamId = awayTeamId,
            SeasonId = seasonId,
            MatchDate = matchDate,
            Matchday = matchday,
            Status = MatchStatus.Scheduled
        };
    }

    public void RecordResult(int homeGoals, int awayGoals, int? attendance = null)
    {
        if (homeGoals < 0 || awayGoals < 0)
            throw new DomainException("Goals cannot be negative.");
        if (Status == MatchStatus.Completed)
            throw new DomainException("Cannot update result of an already completed match.");

        HomeGoals = homeGoals;
        AwayGoals = awayGoals;
        AttendanceCount = attendance;
        Status = MatchStatus.Completed;
        SetUpdated();
    }

    public void Postpone()
    {
        if (Status != MatchStatus.Scheduled)
            throw new DomainException("Only scheduled matches can be postponed.");
        Status = MatchStatus.Postponed;
        SetUpdated();
    }
}
