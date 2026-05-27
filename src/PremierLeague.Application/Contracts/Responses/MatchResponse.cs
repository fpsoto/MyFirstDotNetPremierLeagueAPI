namespace PremierLeague.Application.Contracts.Responses;

public record MatchResponse(
    Guid Id,
    Guid HomeTeamId,
    string HomeTeamName,
    Guid AwayTeamId,
    string AwayTeamName,
    int? HomeGoals,
    int? AwayGoals,
    DateTime MatchDate,
    int Matchday,
    string Status,
    string? Result,
    int? AttendanceCount);
