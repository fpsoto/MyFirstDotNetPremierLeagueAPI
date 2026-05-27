using MediatR;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Players.Queries.GetTopScorers;

public sealed class GetTopScorersQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetTopScorersQuery, IReadOnlyList<TopScorerResponse>>
{
    public async Task<IReadOnlyList<TopScorerResponse>> Handle(GetTopScorersQuery request, CancellationToken cancellationToken)
    {
        var players = await uow.Players.GetTopScorersAsync(request.SeasonId, request.Take, cancellationToken);

        return players
            .Where(p => p.Statistic is not null)
            .Select(p => new TopScorerResponse(
                p.Id, p.FullName, p.Nationality, p.Position.ToString(),
                p.Team?.Name ?? string.Empty,
                p.Statistic!.Goals, p.Statistic.Assists, p.Statistic.Appearances))
            .ToList();
    }
}
