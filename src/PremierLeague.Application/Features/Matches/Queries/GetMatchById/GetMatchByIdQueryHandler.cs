using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Matches.Queries.GetMatchById;

public sealed class GetMatchByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMatchByIdQuery, Result<MatchResponse>>
{
    public async Task<Result<MatchResponse>> Handle(GetMatchByIdQuery request, CancellationToken cancellationToken)
    {
        var match = await uow.Matches.GetByIdWithTeamsAsync(request.MatchId, cancellationToken);

        if (match is null)
            return Result<MatchResponse>.NotFound($"Match '{request.MatchId}' not found.");

        string? result = match.Status == MatchStatus.Completed
            ? $"{match.HomeGoals}-{match.AwayGoals}"
            : null;

        var response = new MatchResponse(match.Id,
            match.HomeTeamId, match.HomeTeam.Name,
            match.AwayTeamId, match.AwayTeam.Name,
            match.HomeGoals, match.AwayGoals, match.MatchDate, match.Matchday,
            match.Status.ToString(), result, match.AttendanceCount);

        return Result<MatchResponse>.Success(response);
    }
}
