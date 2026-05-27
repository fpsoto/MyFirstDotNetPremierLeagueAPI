namespace PremierLeague.Application.Contracts.Responses;

public record TeamResponse(
    Guid Id,
    string Name,
    string ShortName,
    string Stadium,
    int StadiumCapacity,
    string Coach,
    int FoundedYear,
    string City,
    string PrimaryColor);

public record TeamDetailResponse(
    Guid Id,
    string Name,
    string ShortName,
    string Stadium,
    int StadiumCapacity,
    string Coach,
    int FoundedYear,
    string City,
    string PrimaryColor,
    IReadOnlyList<PlayerSummaryResponse> Players,
    StandingResponse? CurrentStanding);

public record PlayerSummaryResponse(
    Guid Id,
    string FullName,
    string Position,
    int ShirtNumber,
    string Nationality);
