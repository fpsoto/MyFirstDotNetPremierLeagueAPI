using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Enums;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Players.Queries.GetPlayers;

public sealed class GetPlayersQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetPlayersQuery, PaginatedResult<PlayerResponse>>
{
    public async Task<PaginatedResult<PlayerResponse>> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
    {
        PlayerPosition? position = request.Position is not null
            ? Enum.Parse<PlayerPosition>(request.Position, ignoreCase: true)
            : null;

        var players = await uow.Players.GetFilteredAsync(
            request.TeamId, position, request.Nationality,
            request.PageNumber, request.PageSize, cancellationToken);

        var totalCount = await uow.Players.GetFilteredCountAsync(
            request.TeamId, position, request.Nationality, cancellationToken);

        var items = players
            .Where(p => request.Search is null ||
                        p.FullName.Contains(request.Search, StringComparison.OrdinalIgnoreCase))
            .Select(p => new PlayerResponse(
                p.Id, p.FullName, p.FirstName, p.LastName, p.Age, p.Nationality,
                p.Position.ToString(), p.ShirtNumber, p.MarketValueMillions,
                p.TeamId, p.Team?.Name ?? string.Empty))
            .ToList();

        return PaginatedResult<PlayerResponse>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }
}
