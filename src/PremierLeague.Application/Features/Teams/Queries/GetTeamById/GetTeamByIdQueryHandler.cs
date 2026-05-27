using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Teams.Queries.GetTeamById;

public sealed class GetTeamByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetTeamByIdQuery, Result<TeamDetailResponse>>
{
    public async Task<Result<TeamDetailResponse>> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
    {
        var team = await uow.Teams.GetByIdWithPlayersAsync(request.TeamId, cancellationToken);

        if (team is null)
            return Result<TeamDetailResponse>.NotFound($"Team '{request.TeamId}' not found.");

        var standing = await uow.Standings.GetByTeamAndSeasonAsync(request.TeamId, request.SeasonId, cancellationToken);

        var standingResponse = standing is not null
            ? new StandingResponse(standing.Position, standing.TeamId, team.Name, team.City,
                standing.Played, standing.Won, standing.Drawn, standing.Lost,
                standing.GoalsFor, standing.GoalsAgainst, standing.GoalDifference, standing.Points)
            : null;

        var players = team.Players
            .Select(p => new PlayerSummaryResponse(p.Id, p.FullName, p.Position.ToString(), p.ShirtNumber, p.Nationality))
            .OrderBy(p => p.ShirtNumber)
            .ToList();

        var response = new TeamDetailResponse(
            team.Id, team.Name, team.ShortName, team.Stadium, team.StadiumCapacity,
            team.Coach, team.FoundedYear, team.City, team.PrimaryColor,
            players, standingResponse);

        return Result<TeamDetailResponse>.Success(response);
    }
}
