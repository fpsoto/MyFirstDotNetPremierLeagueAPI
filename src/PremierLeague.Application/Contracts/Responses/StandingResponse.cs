namespace PremierLeague.Application.Contracts.Responses;

public record StandingResponse(
    int Position,
    Guid TeamId,
    string TeamName,
    string City,
    int Played,
    int Won,
    int Drawn,
    int Lost,
    int GoalsFor,
    int GoalsAgainst,
    int GoalDifference,
    int Points);

public record LeagueStatisticsResponse(
    int TotalMatches,
    int TotalGoals,
    double AverageGoalsPerMatch,
    int TotalCleanSheets,
    TopScorerResponse? TopScorer,
    TopAssistResponse? TopAssister,
    string? MostWinsTeam,
    string? BestDefenseTeam,
    string? BestAttackTeam);
