using MediatR;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Teams.Queries.GetBestAttack;

public sealed class GetBestAttackQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetBestAttackQuery, IReadOnlyList<StandingResponse>>
{
    public async Task<IReadOnlyList<StandingResponse>> Handle(GetBestAttackQuery request, CancellationToken cancellationToken)
    {
        var teams = await uow.Teams.GetBestAttackAsync(request.SeasonId, request.Take, cancellationToken);

        var responses = new List<StandingResponse>();
        foreach (var team in teams)
        {
            var standing = await uow.Standings.GetByTeamAndSeasonAsync(team.Id, request.SeasonId, cancellationToken);
            if (standing is not null)
                responses.Add(new StandingResponse(standing.Position, team.Id, team.Name, team.City,
                    standing.Played, standing.Won, standing.Drawn, standing.Lost,
                    standing.GoalsFor, standing.GoalsAgainst, standing.GoalDifference, standing.Points));
        }

        return responses.OrderByDescending(r => r.GoalsFor).ToList();
    }
}
