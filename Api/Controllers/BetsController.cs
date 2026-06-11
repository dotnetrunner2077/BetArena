using Applications.Layer.DTOs;
using Applications.Layer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BetsController : ControllerBase
{
    private readonly IBetService _betService;

    public BetsController(IBetService betService)
    {
        _betService = betService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlaceBetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PlaceBet([FromBody] PlaceBetRequest request)
    {
        try
        {
            var response = await _betService.PlaceBetAsync(request);
            return CreatedAtAction(nameof(PlaceBet), new { id = response.BetId }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
