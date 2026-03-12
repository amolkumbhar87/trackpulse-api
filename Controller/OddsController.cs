using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/odds")]
public class OddsController : ControllerBase
{
    private readonly IHubContext<OddsHub> _hubContext;

    public OddsController(IHubContext<OddsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateOdds(OddsModel odds)
    {
        // Broadcast the updated odds to all clients in the
        await _hubContext.Clients.Group($"race-{odds.RaceId}").SendAsync("ReceiveOddsUpdate", odds);
        return Ok();
    }
    
    }