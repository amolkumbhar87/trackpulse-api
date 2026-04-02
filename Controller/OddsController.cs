using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/odds")]
public class OddsController : ControllerBase
{
    private readonly IHubContext<OddsHub> _hubContext;

    private readonly IOddsRepository _oddsRepository;

    public OddsController(IHubContext<OddsHub> hubContext, 
    IOddsRepository oddsRepository)
    {
        _hubContext = hubContext;
        _oddsRepository = oddsRepository;
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateOdds(OddsModel odds)
    {
        // Broadcast the updated odds to all clients in the
        await _hubContext.Clients.Group($"race-{odds.RaceId}").SendAsync("ReceiveOddsUpdate", odds);
        return Ok();
    }

    [HttpPost("bulk-update")]
public async Task<IActionResult> BulkUpdateOdds(BulkOddsUpdateRequest request)
{
    // 1. Persist to DB first
    await _oddsRepository.BulkUpdateOddsAsync(request.Horses);

    // 2. Broadcast each horse to the race group
    foreach (var horse in request.Horses)
    {
        await _hubContext.Clients
            .Group($"race-{request.RaceId}")
            .SendAsync("ReceiveOddsUpdate", new {
                raceId      = request.RaceId,
                raceHorseId = horse.RaceHorseId,
                winOdds     = horse.WinOdds,
                placeOdds   = horse.PlaceOdds
            });
    }
    return Ok();
}
    
    }