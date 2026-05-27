using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Matches.Queries.GetMatches;

public record GetMatchesQuery(
    Guid SeasonId,
    Guid? TeamId,
    string? Status,
    int PageNumber,
    int PageSize) : IRequest<PaginatedResult<MatchResponse>>;
