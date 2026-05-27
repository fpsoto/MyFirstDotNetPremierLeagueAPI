using MediatR;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Matches.Queries.GetRecentMatches;

public sealed class GetRecentMatchesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetRecentMatchesQuery, IReadOnlyList<MatchResponse>>
{
    public async Task<IReadOnlyList<MatchResponse>> Handle(GetRecentMatchesQuery request, CancellationToken cancellationToken)
    {
        var matches = await uow.Matches.GetRecentAsync(request.SeasonId, request.Take, cancellationToken);

        return matches.Select(m => new MatchResponse(m.Id,
            m.HomeTeamId, m.HomeTeam?.Name ?? string.Empty,
            m.AwayTeamId, m.AwayTeam?.Name ?? string.Empty,
            m.HomeGoals, m.AwayGoals, m.MatchDate, m.Matchday,
            m.Status.ToString(),
            m.Status == MatchStatus.Completed ? $"{m.HomeGoals}-{m.AwayGoals}" : null,
            m.AttendanceCount)).ToList();
    }
}
