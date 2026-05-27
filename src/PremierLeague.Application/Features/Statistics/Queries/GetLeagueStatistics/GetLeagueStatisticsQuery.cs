using MediatR;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Statistics.Queries.GetLeagueStatistics;

public record GetLeagueStatisticsQuery(Guid SeasonId) : IRequest<LeagueStatisticsResponse>;
