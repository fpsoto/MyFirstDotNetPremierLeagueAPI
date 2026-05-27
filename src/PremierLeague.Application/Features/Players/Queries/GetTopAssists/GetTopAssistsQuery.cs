using MediatR;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Players.Queries.GetTopAssists;

public record GetTopAssistsQuery(Guid SeasonId, int Take = 10) : IRequest<IReadOnlyList<TopAssistResponse>>;
