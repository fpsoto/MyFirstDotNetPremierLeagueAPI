using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Interfaces;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Seeders;

public class PlayerStatisticSeeder(AppDbContext context)
{
    public async Task SeedAsync(List<Player> players, List<Match> matches, Season season, CancellationToken cancellationToken = default)
    {
        var random = new Random(42);

        // Pre-compute team goal totals from completed matches
        var teamGoalsScored = new Dictionary<Guid, int>();
        foreach (var match in matches.Where(m => m.Status == MatchStatus.Completed))
        {
            teamGoalsScored.TryAdd(match.HomeTeamId, 0);
            teamGoalsScored.TryAdd(match.AwayTeamId, 0);
            teamGoalsScored[match.HomeTeamId] += match.HomeGoals!.Value;
            teamGoalsScored[match.AwayTeamId] += match.AwayGoals!.Value;
        }

        var completedCount = matches.Count(m => m.Status == MatchStatus.Completed);
        var matchesPerTeam = completedCount > 0 ? (double)completedCount / 20 : 1;

        var statistics = new List<PlayerStatistic>();

        // Group players by team so we can distribute goals coherently
        var playersByTeam = players.GroupBy(p => p.TeamId).ToDictionary(g => g.Key, g => g.ToList());

        foreach (var (teamId, teamPlayers) in playersByTeam)
        {
            if (!teamGoalsScored.TryGetValue(teamId, out var totalGoals)) continue;

            int teamAssists = (int)(totalGoals * 0.75); // ~75% of goals have a credited assist

            // Goal weights by position
            var forwards = teamPlayers.Where(p => p.Position == PlayerPosition.Forward).ToList();
            var midfielders = teamPlayers.Where(p => p.Position == PlayerPosition.Midfielder).ToList();
            var defenders = teamPlayers.Where(p => p.Position == PlayerPosition.Defender).ToList();
            var goalkeepers = teamPlayers.Where(p => p.Position == PlayerPosition.Goalkeeper).ToList();

            var goalBuckets = DistributeGoals(totalGoals, forwards.Count, midfielders.Count, defenders.Count);
            var assistBuckets = DistributeGoals(teamAssists, midfielders.Count, forwards.Count, defenders.Count);

            int fwdIdx = 0, midIdx = 0, defIdx = 0;

            foreach (var player in teamPlayers)
            {
                var appearances = (int)(matchesPerTeam * (0.5 + random.NextDouble() * 0.5));
                appearances = Math.Max(1, Math.Min(appearances, (int)matchesPerTeam));

                int goals, assists, cleanSheets = 0, goalsConceded = 0;
                int yellowCards = random.Next(0, 10);
                int redCards = random.NextDouble() < 0.05 ? 1 : 0;
                int minutesPlayed = appearances * random.Next(60, 92);

                switch (player.Position)
                {
                    case PlayerPosition.Forward:
                        goals = fwdIdx < goalBuckets.Forwards.Count ? goalBuckets.Forwards[fwdIdx++] : 0;
                        assists = random.Next(0, 8);
                        break;
                    case PlayerPosition.Midfielder:
                        goals = midIdx < goalBuckets.Midfielders.Count ? goalBuckets.Midfielders[midIdx++] : 0;
                        assists = midIdx < assistBuckets.Forwards.Count ? assistBuckets.Forwards[midIdx] : random.Next(0, 10);
                        break;
                    case PlayerPosition.Defender:
                        goals = defIdx < goalBuckets.Defenders.Count ? goalBuckets.Defenders[defIdx++] : 0;
                        assists = random.Next(0, 5);
                        break;
                    default: // Goalkeeper
                        goals = 0;
                        assists = 0;
                        cleanSheets = random.Next(0, (int)(matchesPerTeam * 0.4));
                        goalsConceded = totalGoals > 0 ? random.Next(totalGoals / 2, totalGoals) : 0;
                        yellowCards = random.Next(0, 3);
                        break;
                }

                var stat = PlayerStatistic.Create(player.Id, season.Id);
                stat.Update(goals, assists, appearances, yellowCards, redCards, minutesPlayed, cleanSheets, goalsConceded);
                statistics.Add(stat);
            }
        }

        context.PlayerStatistics.AddRange(statistics);
        await context.SaveChangesAsync(cancellationToken);
    }

    // Distributes `total` goals across positions using PL-realistic splits
    private static (List<int> Forwards, List<int> Midfielders, List<int> Defenders) DistributeGoals(
        int total, int fwdCount, int midCount, int defCount)
    {
        int fwdGoals = (int)(total * 0.58);
        int midGoals = (int)(total * 0.32);
        int defGoals = total - fwdGoals - midGoals;

        return (SplitGoals(fwdGoals, fwdCount), SplitGoals(midGoals, midCount), SplitGoals(defGoals, defCount));
    }

    // Splits a goal total into a realistic star-player-heavy distribution
    private static List<int> SplitGoals(int total, int count)
    {
        if (count == 0 || total == 0) return [];

        var result = new List<int>(new int[count]);
        int remaining = total;
        int idx = 0;

        while (remaining > 0)
        {
            int chunk = idx == 0 ? Math.Max(1, (int)(remaining * 0.4)) : Math.Max(1, remaining / (count - idx));
            result[idx] += chunk;
            remaining -= chunk;
            idx = (idx + 1) % count;
        }

        return result.OrderByDescending(g => g).ToList();
    }
}
