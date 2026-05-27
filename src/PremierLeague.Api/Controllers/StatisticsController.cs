using MediatR;
using Microsoft.AspNetCore.Mvc;
using PremierLeague.Application.Features.Statistics.Queries.GetLeagueStatistics;

namespace PremierLeague.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StatisticsController(IMediator mediator) : ControllerBase
{
    [HttpGet("league")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLeagueStatistics(
        [FromQuery] Guid seasonId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetLeagueStatisticsQuery(seasonId), cancellationToken);
        return Ok(result);
    }
}
