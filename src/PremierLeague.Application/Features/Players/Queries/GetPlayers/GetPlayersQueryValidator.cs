using FluentValidation;
using PremierLeague.Domain.Enums;

namespace PremierLeague.Application.Features.Players.Queries.GetPlayers;

public class GetPlayersQueryValidator : AbstractValidator<GetPlayersQuery>
{
    public GetPlayersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Position)
            .Must(p => p is null || Enum.TryParse<PlayerPosition>(p, ignoreCase: true, out _))
            .WithMessage("Position must be: Goalkeeper, Defender, Midfielder, or Forward.");
    }
}
