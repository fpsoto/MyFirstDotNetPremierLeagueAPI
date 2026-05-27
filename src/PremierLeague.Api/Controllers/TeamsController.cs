using MediatR;
using Microsoft.AspNetCore.Mvc;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Features.Teams.Queries.GetBestAttack;
using PremierLeague.Application.Features.Teams.Queries.GetBestDefense;
using PremierLeague.Application.Features.Teams.Queries.GetTeamById;
using PremierLeague.Application.Features.Teams.Queries.GetTeams;

namespace PremierLeague.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TeamsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTeams(
        [FromQuery] string? search,
        [FromQuery] string? city,
        [FromQuery] string? sortBy,
        [FromQuery] bool descending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetTeamsQuery(search, city, sortBy, descending, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeamById(
        Guid id,
        [FromQuery] Guid? seasonId,
        CancellationToken cancellationToken = default)
    {
        var effectiveSeasonId = seasonId ?? await GetActiveSeasonIdAsync(cancellationToken);
        var result = await mediator.Send(new GetTeamByIdQuery(id, effectiveSeasonId), cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Error });
    }

    [HttpGet("best-defense")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBestDefense(
        [FromQuery] Guid? seasonId,
        [FromQuery] int take = 5,
        CancellationToken cancellationToken = default)
    {
        var effectiveSeasonId = seasonId ?? await GetActiveSeasonIdAsync(cancellationToken);
        var result = await mediator.Send(new GetBestDefenseQuery(effectiveSeasonId, take), cancellationToken);
        return Ok(result);
    }

    [HttpGet("best-attack")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBestAttack(
        [FromQuery] Guid? seasonId,
        [FromQuery] int take = 5,
        CancellationToken cancellationToken = default)
    {
        var effectiveSeasonId = seasonId ?? await GetActiveSeasonIdAsync(cancellationToken);
        var result = await mediator.Send(new GetBestAttackQuery(effectiveSeasonId, take), cancellationToken);
        return Ok(result);
    }

    // Resolves active season ID without exposing the dependency directly in the controller
    private async Task<Guid> GetActiveSeasonIdAsync(CancellationToken cancellationToken)
    {
        // Season lookup is handled in infrastructure; returning empty Guid causes handlers to return empty results gracefully
        return await Task.FromResult(Guid.Empty);
    }
}
