namespace PremierLeague.Application.Contracts.Responses;

public record PlayerResponse(
    Guid Id,
    string FullName,
    string FirstName,
    string LastName,
    int Age,
    string Nationality,
    string Position,
    int ShirtNumber,
    decimal MarketValueMillions,
    Guid TeamId,
    string TeamName);

public record PlayerDetailResponse(
    Guid Id,
    string FullName,
    string FirstName,
    string LastName,
    int Age,
    string Nationality,
    string Position,
    int ShirtNumber,
    decimal MarketValueMillions,
    Guid TeamId,
    string TeamName,
    PlayerStatisticResponse? Statistics);

public record PlayerStatisticResponse(
    int Goals,
    int Assists,
    int Appearances,
    int YellowCards,
    int RedCards,
    int MinutesPlayed,
    int CleanSheets,
    double GoalsPerGame,
    double AssistsPerGame);

public record TopScorerResponse(
    Guid PlayerId,
    string FullName,
    string Nationality,
    string Position,
    string TeamName,
    int Goals,
    int Assists,
    int Appearances);

public record TopAssistResponse(
    Guid PlayerId,
    string FullName,
    string Nationality,
    string Position,
    string TeamName,
    int Assists,
    int Goals,
    int Appearances);
