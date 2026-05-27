using MediatR;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Standings.Queries.GetStandings;

public record GetStandingsQuery(Guid SeasonId) : IRequest<IReadOnlyList<StandingResponse>>;
