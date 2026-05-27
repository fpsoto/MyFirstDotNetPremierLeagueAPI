using MediatR;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Teams.Queries.GetBestDefense;

public record GetBestDefenseQuery(Guid SeasonId, int Take = 5) : IRequest<IReadOnlyList<StandingResponse>>;
