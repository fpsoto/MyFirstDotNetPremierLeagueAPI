using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Players.Queries.GetPlayerById;

public record GetPlayerByIdQuery(Guid PlayerId, Guid SeasonId) : IRequest<Result<PlayerDetailResponse>>;
