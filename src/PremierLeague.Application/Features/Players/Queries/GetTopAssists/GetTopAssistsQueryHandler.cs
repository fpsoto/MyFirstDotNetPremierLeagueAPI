using MediatR;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Players.Queries.GetTopAssists;

public sealed class GetTopAssistsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetTopAssistsQuery, IReadOnlyList<TopAssistResponse>>
{
    public async Task<IReadOnlyList<TopAssistResponse>> Handle(GetTopAssistsQuery request, CancellationToken cancellationToken)
    {
        var players = await uow.Players.GetTopAssistsAsync(request.SeasonId, request.Take, cancellationToken);

        return players
            .Where(p => p.Statistic is not null)
            .Select(p => new TopAssistResponse(
                p.Id, p.FullName, p.Nationality, p.Position.ToString(),
                p.Team?.Name ?? string.Empty,
                p.Statistic!.Assists, p.Statistic.Goals, p.Statistic.Appearances))
            .ToList();
    }
}
