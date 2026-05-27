using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Players.Queries.GetPlayerById;

public sealed class GetPlayerByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetPlayerByIdQuery, Result<PlayerDetailResponse>>
{
    public async Task<Result<PlayerDetailResponse>> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
    {
        var player = await uow.Players.GetByIdWithStatisticsAsync(request.PlayerId, request.SeasonId, cancellationToken);

        if (player is null)
            return Result<PlayerDetailResponse>.NotFound($"Player '{request.PlayerId}' not found.");

        PlayerStatisticResponse? statsResponse = null;
        if (player.Statistic is not null)
        {
            var s = player.Statistic;
            statsResponse = new PlayerStatisticResponse(s.Goals, s.Assists, s.Appearances,
                s.YellowCards, s.RedCards, s.MinutesPlayed, s.CleanSheets, s.GoalsPerGame, s.AssistsPerGame);
        }

        var response = new PlayerDetailResponse(
            player.Id, player.FullName, player.FirstName, player.LastName,
            player.Age, player.Nationality, player.Position.ToString(),
            player.ShirtNumber, player.MarketValueMillions,
            player.TeamId, player.Team?.Name ?? string.Empty,
            statsResponse);

        return Result<PlayerDetailResponse>.Success(response);
    }
}
