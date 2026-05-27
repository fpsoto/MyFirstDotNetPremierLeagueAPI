using Bogus;
using PremierLeague.Domain.Entities;
using PremierLeague.Domain.Enums;
using PremierLeague.Infrastructure.Persistence;

namespace PremierLeague.Infrastructure.Seeders;

public class PlayerSeeder(AppDbContext context)
{
    private static readonly string[] Nationalities =
    [
        "English", "Spanish", "French", "German", "Brazilian", "Argentine",
        "Portuguese", "Dutch", "Belgian", "Italian", "Norwegian", "Danish",
        "Swedish", "Scottish", "Irish", "Welsh", "Senegalese", "Ivorian",
        "Ghanaian", "Nigerian", "Moroccan", "Egyptian", "Colombian", "Mexican",
        "Uruguayan", "Chilean", "Japanese", "South Korean", "Australian", "American"
    ];

    // Position distribution per squad: 2GK, 6DEF, 8MID, 5FWD = 21 players
    private static readonly (PlayerPosition Position, int Count)[] SquadShape =
    [
        (PlayerPosition.Goalkeeper, 2),
        (PlayerPosition.Defender, 6),
        (PlayerPosition.Midfielder, 8),
        (PlayerPosition.Forward, 5)
    ];

    public async Task<List<Player>> SeedAsync(List<Team> teams, CancellationToken cancellationToken = default)
    {
        var faker = new Faker();
        var random = new Random(42); // fixed seed for reproducibility
        var players = new List<Player>();

        foreach (var team in teams)
        {
            var usedShirtNumbers = new HashSet<int>();
            int slotNumber = 1;

            foreach (var (position, count) in SquadShape)
            {
                for (int i = 0; i < count; i++)
                {
                    // Ensure unique shirt numbers within a team
                    int shirtNumber;
                    if (slotNumber <= 11)
                    {
                        shirtNumber = slotNumber;
                    }
                    else
                    {
                        do { shirtNumber = random.Next(12, 50); }
                        while (!usedShirtNumbers.Add(shirtNumber));
                    }
                    usedShirtNumbers.Add(shirtNumber);

                    var age = position == PlayerPosition.Goalkeeper
                        ? random.Next(20, 38)
                        : random.Next(18, 35);

                    var marketValue = position switch
                    {
                        PlayerPosition.Forward => (decimal)random.Next(10, 120),
                        PlayerPosition.Midfielder => (decimal)random.Next(8, 100),
                        PlayerPosition.Defender => (decimal)random.Next(5, 80),
                        _ => (decimal)random.Next(3, 40)
                    };

                    var nationality = Nationalities[random.Next(Nationalities.Length)];

                    var player = Player.Create(
                        faker.Name.FirstName(),
                        faker.Name.LastName(),
                        age,
                        nationality,
                        position,
                        slotNumber <= 11 ? slotNumber : shirtNumber,
                        marketValue,
                        team.Id);

                    players.Add(player);
                    slotNumber++;
                }
            }
        }

        context.Players.AddRange(players);
        await context.SaveChangesAsync(cancellationToken);
        return players;
    }
}
