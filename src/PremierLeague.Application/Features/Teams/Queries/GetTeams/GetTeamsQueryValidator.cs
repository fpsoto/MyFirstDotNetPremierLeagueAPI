using FluentValidation;

namespace PremierLeague.Application.Features.Teams.Queries.GetTeams;

public class GetTeamsQueryValidator : AbstractValidator<GetTeamsQuery>
{
    private static readonly string[] AllowedSortFields = ["name", "city", "foundedyear", "stadiumcapacity"];

    public GetTeamsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0.");
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
        RuleFor(x => x.SortBy)
            .Must(s => s is null || AllowedSortFields.Contains(s.ToLower()))
            .WithMessage($"SortBy must be one of: {string.Join(", ", AllowedSortFields)}.");
    }
}
