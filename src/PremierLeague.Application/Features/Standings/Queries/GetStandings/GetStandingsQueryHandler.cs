using MediatR;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Standings.Queries.GetStandings;

public sealed class GetStandingsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetStandingsQuery, IReadOnlyList<StandingResponse>>
{
    public async Task<IReadOnlyList<StandingResponse>> Handle(GetStandingsQuery request, CancellationToken cancellationToken)
    {
        var standings = await uow.Standings.GetBySeasonAsync(request.SeasonId, cancellationToken);

        return standings
            .OrderBy(s => s.Position)
            .Select(s => new StandingResponse(
                s.Position, s.TeamId, s.Team?.Name ?? string.Empty, s.Team?.City ?? string.Empty,
                s.Played, s.Won, s.Drawn, s.Lost,
                s.GoalsFor, s.GoalsAgainst, s.GoalDifference, s.Points))
            .ToList();
    }
}
