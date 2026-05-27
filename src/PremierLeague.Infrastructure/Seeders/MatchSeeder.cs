using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Enums;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Seeders;

public class MatchSeeder(AppDbContext context)
{
    // Weight distribution mirrors real PL scoring patterns
    private static readonly (int Home, int Away, int Weight)[] ScoreWeights =
    [
        (0, 0, 8), (1, 0, 14), (0, 1, 11), (1, 1, 14),
        (2, 0, 11), (0, 2, 8),  (2, 1, 13), (1, 2, 10),
        (3, 0, 5),  (0, 3, 4),  (3, 1, 6),  (1, 3, 5),
        (2, 2, 5),  (3, 2, 3),  (2, 3, 3),  (4, 0, 2),
        (0, 4, 1),  (4, 1, 2),  (1, 4, 1),  (4, 2, 1)
    ];

    private static readonly int TotalWeight = ScoreWeights.Sum(s => s.Weight);

    public async Task<List<Match>> SeedAsync(List<Team> teams, Season season, CancellationToken cancellationToken = default)
    {
        var random = new Random(42);
        var matches = new List<Match>();
        var seasonStart = new DateTime(2024, 8, 17, 15, 0, 0, DateTimeKind.Utc);

        // Build the round-robin fixture list: each pair plays home & away
        var fixtures = BuildRoundRobin(teams);
        int matchday = 1;
        int matchesPerRound = teams.Count / 2;

        for (int round = 0; round < fixtures.Count; round++)
        {
            if (round > 0 && round % matchesPerRound == 0) matchday++;

            var (home, away) = fixtures[round];
            var matchDate = seasonStart.AddDays((matchday - 1) * 7).AddHours(random.Next(-2, 3));

            var match = Match.Schedule(home.Id, away.Id, season.Id, matchDate, matchday);

            // Completed matches are those already played (before today)
            if (matchDate < DateTime.UtcNow)
            {
                var (homeGoals, awayGoals) = PickScore(random);
                var minAttendance = Math.Min(5000, home.StadiumCapacity - 1);
                var attendance = random.Next(minAttendance, home.StadiumCapacity);
                match.RecordResult(homeGoals, awayGoals, attendance);
            }

            matches.Add(match);
        }

        context.Matches.AddRange(matches);
        await context.SaveChangesAsync(cancellationToken);
        return matches;
    }

    private static List<(Team Home, Team Away)> BuildRoundRobin(List<Team> teams)
    {
        var fixtures = new List<(Team, Team)>();
        int n = teams.Count;
        var circle = teams.Skip(1).ToList();
        var pivot = teams[0];

        for (int round = 0; round < n - 1; round++)
        {
            var roundTeams = new List<Team> { pivot };
            roundTeams.AddRange(circle);

            for (int i = 0; i < n / 2; i++)
            {
                fixtures.Add((roundTeams[i], roundTeams[n - 1 - i]));
            }

            // Rotate circle for next round
            circle.Insert(0, circle[^1]);
            circle.RemoveAt(circle.Count - 1);
        }

        // Second leg: swap home and away
        var firstLeg = fixtures.ToList();
        fixtures.AddRange(firstLeg.Select(f => (f.Item2, f.Item1)));

        return fixtures;
    }

    private static (int Home, int Away) PickScore(Random random)
    {
        int roll = random.Next(TotalWeight);
        int cumulative = 0;
        foreach (var (home, away, weight) in ScoreWeights)
        {
            cumulative += weight;
            if (roll < cumulative) return (home, away);
        }
        return (1, 1);
    }
}
