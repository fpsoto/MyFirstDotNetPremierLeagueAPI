using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Teams.Queries.GetTeams;

public record GetTeamsQuery(
    string? Search,
    string? City,
    string? SortBy,
    bool Descending,
    int PageNumber,
    int PageSize) : IRequest<PaginatedResult<TeamResponse>>;
