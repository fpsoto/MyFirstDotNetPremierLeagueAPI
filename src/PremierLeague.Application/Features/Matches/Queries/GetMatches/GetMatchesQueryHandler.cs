using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Matches.Queries.GetMatches;

public sealed class GetMatchesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMatchesQuery, PaginatedResult<MatchResponse>>
{
    public async Task<PaginatedResult<MatchResponse>> Handle(GetMatchesQuery request, CancellationToken cancellationToken)
    {
        MatchStatus? status = request.Status is not null
            ? Enum.Parse<MatchStatus>(request.Status, ignoreCase: true)
            : null;

        var matches = await uow.Matches.GetFilteredAsync(
            request.SeasonId, request.TeamId, status,
            request.PageNumber, request.PageSize, cancellationToken);

        var totalCount = await uow.Matches.GetFilteredCountAsync(
            request.SeasonId, request.TeamId, status, cancellationToken);

        var items = matches.Select(ToResponse).ToList();

        return PaginatedResult<MatchResponse>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    private static MatchResponse ToResponse(Domain.Entities.Match m)
    {
        string? result = null;
        if (m.Status == MatchStatus.Completed)
            result = $"{m.HomeGoals}-{m.AwayGoals}";

        return new MatchResponse(m.Id,
            m.HomeTeamId, m.HomeTeam?.Name ?? string.Empty,
            m.AwayTeamId, m.AwayTeam?.Name ?? string.Empty,
            m.HomeGoals, m.AwayGoals, m.MatchDate, m.Matchday,
            m.Status.ToString(), result, m.AttendanceCount);
    }
}
