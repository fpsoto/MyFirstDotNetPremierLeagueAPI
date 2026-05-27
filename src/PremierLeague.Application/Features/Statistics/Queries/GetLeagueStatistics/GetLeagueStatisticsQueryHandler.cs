using MediatR;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Statistics.Queries.GetLeagueStatistics;

public sealed class GetLeagueStatisticsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetLeagueStatisticsQuery, LeagueStatisticsResponse>
{
    public async Task<LeagueStatisticsResponse> Handle(GetLeagueStatisticsQuery request, CancellationToken cancellationToken)
    {
        var matches = await uow.Matches.GetBySeasonAsync(request.SeasonId, cancellationToken);
        var standings = await uow.Standings.GetBySeasonAsync(request.SeasonId, cancellationToken);
        var topScorers = await uow.Players.GetTopScorersAsync(request.SeasonId, 1, cancellationToken);
        var topAssists = await uow.Players.GetTopAssistsAsync(request.SeasonId, 1, cancellationToken);

        var completedMatches = matches.Where(m => m.Status == MatchStatus.Completed).ToList();
        var totalGoals = completedMatches.Sum(m => (m.HomeGoals ?? 0) + (m.AwayGoals ?? 0));
        var avgGoals = completedMatches.Count > 0
            ? Math.Round((double)totalGoals / completedMatches.Count, 2)
            : 0;

        var topScorer = topScorers.FirstOrDefault(p => p.Statistic is not null);
        var topAssister = topAssists.FirstOrDefault(p => p.Statistic is not null);

        var bestDefense = standings.OrderBy(s => s.GoalsAgainst).FirstOrDefault();
        var bestAttack = standings.OrderByDescending(s => s.GoalsFor).FirstOrDefault();
        var mostWins = standings.OrderByDescending(s => s.Won).FirstOrDefault();

        TopScorerResponse? scorerResponse = topScorer is not null
            ? new TopScorerResponse(topScorer.Id, topScorer.FullName, topScorer.Nationality,
                topScorer.Position.ToString(), topScorer.Team?.Name ?? string.Empty,
                topScorer.Statistic!.Goals, topScorer.Statistic.Assists, topScorer.Statistic.Appearances)
            : null;

        TopAssistResponse? assisterResponse = topAssister is not null
            ? new TopAssistResponse(topAssister.Id, topAssister.FullName, topAssister.Nationality,
                topAssister.Position.ToString(), topAssister.Team?.Name ?? string.Empty,
                topAssister.Statistic!.Assists, topAssister.Statistic.Goals, topAssister.Statistic.Appearances)
            : null;

        return new LeagueStatisticsResponse(
            completedMatches.Count, totalGoals, avgGoals,
            completedMatches.Count(m => m.HomeGoals == 0 || m.AwayGoals == 0),
            scorerResponse, assisterResponse,
            mostWins?.Team?.Name, bestDefense?.Team?.Name, bestAttack?.Team?.Name);
    }
}
