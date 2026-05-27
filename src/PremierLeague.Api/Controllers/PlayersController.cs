using MediatR;
using Microsoft.AspNetCore.Mvc;
using PremierLeague.Application.Features.Players.Queries.GetPlayerById;
using PremierLeague.Application.Features.Players.Queries.GetPlayers;
using PremierLeague.Application.Features.Players.Queries.GetTopAssists;
using PremierLeague.Application.Features.Players.Queries.GetTopScorers;

namespace PremierLeague.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PlayersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPlayers(
        [FromQuery] Guid? teamId,
        [FromQuery] string? position,
        [FromQuery] string? nationality,
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetPlayersQuery(teamId, position, nationality, search, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlayerById(
        Guid id,
        [FromQuery] Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetPlayerByIdQuery(id, seasonId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error });
    }

    [HttpGet("top-scorers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopScorers(
        [FromQuery] Guid seasonId,
        [FromQuery] int take = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetTopScorersQuery(seasonId, take), cancellationToken);
        return Ok(result);
    }

    [HttpGet("top-assists")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopAssists(
        [FromQuery] Guid seasonId,
        [FromQuery] int take = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetTopAssistsQuery(seasonId, take), cancellationToken);
        return Ok(result);
    }
}
