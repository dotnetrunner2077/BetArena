using Applications.Layer.DTOs;
using Applications.Layer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;

    public StatsController(IStatsService statsService)
    {
        _statsService = statsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(StatsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _statsService.GetStatsAsync();
        return Ok(stats);
    }
}
