using MediatR;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Matches.Queries.GetUpcomingMatches;

public sealed class GetUpcomingMatchesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetUpcomingMatchesQuery, IReadOnlyList<MatchResponse>>
{
    public async Task<IReadOnlyList<MatchResponse>> Handle(GetUpcomingMatchesQuery request, CancellationToken cancellationToken)
    {
        var matches = await uow.Matches.GetUpcomingAsync(request.SeasonId, request.Take, cancellationToken);

        return matches.Select(m => new MatchResponse(m.Id,
            m.HomeTeamId, m.HomeTeam?.Name ?? string.Empty,
            m.AwayTeamId, m.AwayTeam?.Name ?? string.Empty,
            m.HomeGoals, m.AwayGoals, m.MatchDate, m.Matchday,
            m.Status.ToString(), null, m.AttendanceCount)).ToList();
    }
}
