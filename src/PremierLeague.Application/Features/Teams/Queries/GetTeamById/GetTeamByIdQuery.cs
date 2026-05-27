using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Teams.Queries.GetTeamById;

public record GetTeamByIdQuery(Guid TeamId, Guid SeasonId) : IRequest<Result<TeamDetailResponse>>;
