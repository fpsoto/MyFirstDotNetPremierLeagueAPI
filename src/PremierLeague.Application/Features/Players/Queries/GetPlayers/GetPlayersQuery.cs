using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Players.Queries.GetPlayers;

public record GetPlayersQuery(
    Guid? TeamId,
    string? Position,
    string? Nationality,
    string? Search,
    int PageNumber,
    int PageSize) : IRequest<PaginatedResult<PlayerResponse>>;
