using Bogus;
using PremierLeague.Domain.Entities;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Seeders;

public class TeamSeeder(AppDbContext context)
{
    // Real Premier League 2024-25 clubs
    private static readonly (string Name, string Short, string Stadium, int Cap, string City, string Color, int Founded)[] PremierLeagueTeams =
    [
        ("Arsenal", "ARS", "Emirates Stadium", 60704, "London", "Red", 1886),
        ("Aston Villa", "AVL", "Villa Park", 42785, "Birmingham", "Claret", 1874),
        ("Bournemouth", "BOU", "Vitality Stadium", 11307, "Bournemouth", "Red", 1890),
        ("Brentford", "BRE", "Gtech Community Stadium", 17250, "London", "Red", 1889),
        ("Brighton & Hove Albion", "BHA", "Amex Stadium", 31800, "Brighton", "Blue", 1901),
        ("Chelsea", "CHE", "Stamford Bridge", 40341, "London", "Blue", 1905),
        ("Crystal Palace", "CRY", "Selhurst Park", 25486, "London", "Red", 1905),
        ("Everton", "EVE", "Goodison Park", 39414, "Liverpool", "Blue", 1878),
        ("Fulham", "FUL", "Craven Cottage", 25700, "London", "White", 1879),
        ("Ipswich Town", "IPS", "Portman Road", 29312, "Ipswich", "Blue", 1878),
        ("Leicester City", "LEI", "King Power Stadium", 32261, "Leicester", "Blue", 1884),
        ("Liverpool", "LIV", "Anfield", 61276, "Liverpool", "Red", 1892),
        ("Manchester City", "MCI", "Etihad Stadium", 53400, "Manchester", "Sky Blue", 1880),
        ("Manchester United", "MUN", "Old Trafford", 74310, "Manchester", "Red", 1878),
        ("Newcastle United", "NEW", "St. James Park", 52305, "Newcastle", "Black and White", 1892),
        ("Nottingham Forest", "NFO", "City Ground", 30332, "Nottingham", "Red", 1865),
        ("Southampton", "SOU", "St. Marys Stadium", 32384, "Southampton", "Red", 1885),
        ("Tottenham Hotspur", "TOT", "Tottenham Hotspur Stadium", 62850, "London", "White", 1882),
        ("West Ham United", "WHU", "London Stadium", 62500, "London", "Claret", 1895),
        ("Wolverhampton Wanderers", "WOL", "Molineux", 32050, "Wolverhampton", "Gold", 1877)
    ];

    public async Task<List<Team>> SeedAsync(CancellationToken cancellationToken = default)
    {
        var faker = new Faker();
        var teams = new List<Team>();

        foreach (var (name, shortName, stadium, cap, city, color, founded) in PremierLeagueTeams)
        {
            var coach = faker.Name.FullName();
            var team = Team.Create(name, shortName, stadium, cap, coach, founded, city, color);
            teams.Add(team);
        }

        context.Teams.AddRange(teams);
        await context.SaveChangesAsync(cancellationToken);
        return teams;
    }
}
