using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Exceptions;

namespace PremierLeague.Domain.Entities;

public class Player : BaseEntity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public int Age { get; private set; }
    public string Nationality { get; private set; } = default!;
    public PlayerPosition Position { get; private set; }
    public int ShirtNumber { get; private set; }
    public decimal MarketValueMillions { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Guid TeamId { get; private set; }
    public Team Team { get; private set; } = default!;
    public PlayerStatistic? Statistic { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    private Player() { }

    public static Player Create(
        string firstName, string lastName, int age, string nationality,
        PlayerPosition position, int shirtNumber, decimal marketValueMillions, Guid teamId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("Player first name cannot be empty.");
        if (age < 16 || age > 45)
            throw new DomainException($"Player age {age} is outside the valid range (16-45).");
        if (shirtNumber < 1 || shirtNumber > 99)
            throw new DomainException($"Shirt number {shirtNumber} must be between 1 and 99.");

        return new Player
        {
            FirstName = firstName,
            LastName = lastName,
            Age = age,
            Nationality = nationality,
            Position = position,
            ShirtNumber = shirtNumber,
            MarketValueMillions = marketValueMillions,
            TeamId = teamId
        };
    }

    public void Transfer(Guid newTeamId)
    {
        if (newTeamId == Guid.Empty)
            throw new DomainException("Target team ID is invalid.");
        TeamId = newTeamId;
        SetUpdated();
    }

    public void Retire() { IsActive = false; SetUpdated(); }
}
