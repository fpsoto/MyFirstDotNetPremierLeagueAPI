using MediatR;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Players.Queries.GetTopScorers;

public record GetTopScorersQuery(Guid SeasonId, int Take = 10) : IRequest<IReadOnlyList<TopScorerResponse>>;
