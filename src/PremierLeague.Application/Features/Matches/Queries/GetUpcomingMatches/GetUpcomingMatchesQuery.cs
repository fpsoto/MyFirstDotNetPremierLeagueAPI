using MediatR;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Matches.Queries.GetUpcomingMatches;

public record GetUpcomingMatchesQuery(Guid SeasonId, int Take = 10) : IRequest<IReadOnlyList<MatchResponse>>;
