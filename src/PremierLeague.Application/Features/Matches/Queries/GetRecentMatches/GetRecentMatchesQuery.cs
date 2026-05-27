using MediatR;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Matches.Queries.GetRecentMatches;

public record GetRecentMatchesQuery(Guid SeasonId, int Take = 10) : IRequest<IReadOnlyList<MatchResponse>>;
