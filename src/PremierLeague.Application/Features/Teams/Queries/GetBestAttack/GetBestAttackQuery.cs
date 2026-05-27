using MediatR;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Teams.Queries.GetBestAttack;

public record GetBestAttackQuery(Guid SeasonId, int Take = 5) : IRequest<IReadOnlyList<StandingResponse>>;
