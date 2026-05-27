using MediatR;
using Microsoft.AspNetCore.Mvc;
using PremierLeague.Application.Features.Standings.Queries.GetStandings;

namespace PremierLeague.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StandingsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStandings(
        [FromQuery] Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetStandingsQuery(seasonId), cancellationToken);
        return Ok(result);
    }
}
