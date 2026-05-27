using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Enums;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Seeders;

public class StandingSeeder(AppDbContext context)
{
    public async Task SeedAsync(List<Team> teams, List<Match> matches, Season season, CancellationToken cancellationToken = default)
    {
        var completedMatches = matches.Where(m => m.Status == MatchStatus.Completed).ToList();

        // Aggregate results per team from match data
        var stats = teams.ToDictionary(t => t.Id, _ => new TeamStats());

        foreach (var match in completedMatches)
        {
            var homeStats = stats[match.HomeTeamId];
            var awayStats = stats[match.AwayTeamId];

            homeStats.Played++;
            awayStats.Played++;
            homeStats.GoalsFor += match.HomeGoals!.Value;
            homeStats.GoalsAgainst += match.AwayGoals!.Value;
            awayStats.GoalsFor += match.AwayGoals!.Value;
            awayStats.GoalsAgainst += match.HomeGoals!.Value;

            if (match.IsHomeWin) { homeStats.Won++; awayStats.Lost++; }
            else if (match.IsAwayWin) { awayStats.Won++; homeStats.Lost++; }
            else { homeStats.Drawn++; awayStats.Drawn++; }
        }

        // Rank by points, then goal difference, then goals for
        var ranked = teams
            .Select(t => (Team: t, Stats: stats[t.Id]))
            .OrderByDescending(x => x.Stats.Points)
            .ThenByDescending(x => x.Stats.GoalDifference)
            .ThenByDescending(x => x.Stats.GoalsFor)
            .ToList();

        var standings = new List<LeagueStanding>();
        for (int i = 0; i < ranked.Count; i++)
        {
            var (team, s) = ranked[i];
            var standing = LeagueStanding.Create(team.Id, season.Id);
            standing.Update(i + 1, s.Played, s.Won, s.Drawn, s.Lost, s.GoalsFor, s.GoalsAgainst);
            standings.Add(standing);
        }

        context.Standings.AddRange(standings);
        await context.SaveChangesAsync(cancellationToken);
    }

    private sealed class TeamStats
    {
        public int Played { get; set; }
        public int Won { get; set; }
        public int Drawn { get; set; }
        public int Lost { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int Points => (Won * 3) + Drawn;
        public int GoalDifference => GoalsFor - GoalsAgainst;
    }
}
