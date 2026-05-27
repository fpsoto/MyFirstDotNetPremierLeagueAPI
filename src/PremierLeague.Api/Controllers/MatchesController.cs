using MediatR;
using Microsoft.AspNetCore.Mvc;
using PremierLeague.Application.Features.Matches.Queries.GetMatchById;
using PremierLeague.Application.Features.Matches.Queries.GetMatches;
using PremierLeague.Application.Features.Matches.Queries.GetRecentMatches;
using PremierLeague.Application.Features.Matches.Queries.GetUpcomingMatches;

namespace PremierLeague.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MatchesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMatches(
        [FromQuery] Guid seasonId,
        [FromQuery] Guid? teamId,
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetMatchesQuery(seasonId, teamId, status, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMatchById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetMatchByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error });
    }

    [HttpGet("recent")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentMatches(
        [FromQuery] Guid seasonId,
        [FromQuery] int take = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetRecentMatchesQuery(seasonId, take), cancellationToken);
        return Ok(result);
    }

    [HttpGet("upcoming")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcomingMatches(
        [FromQuery] Guid seasonId,
        [FromQuery] int take = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetUpcomingMatchesQuery(seasonId, take), cancellationToken);
        return Ok(result);
    }
}
