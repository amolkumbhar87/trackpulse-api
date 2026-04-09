using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/[controller]")]
public class RaceController : ControllerBase
{


    private readonly IRaceRepository _raceRepository;

    private readonly IRaceCardRepository _raceCardRepository;

    private readonly IHubContext<OddsHub> _hubContext;

    public RaceController(IRaceRepository raceRepository,
    IRaceCardRepository raceCardRepository,
    IHubContext<OddsHub> hubContext
    )
    {
        _raceRepository = raceRepository;

        _raceCardRepository = raceCardRepository;
        _hubContext = hubContext;

    }


    // [HttpGet("races/{cityName}/{raceDate}")]
    [HttpGet("races")]
    public async Task<IActionResult> GetRaceByCityAndDateAsync(string cityName, string raceDate)
    {
        var races = await _raceRepository.GetRaceByCityAndDateAsync(cityName, raceDate);
        return Ok(races);
    }

    [HttpGet("races-by-date")]
    public async Task<IActionResult> GetRaceByDateAsync(string raceDate)
    {
        var races = await _raceCardRepository.GetRaceByDateAsync(raceDate);
        return Ok(races);
    }

    [HttpGet("races-by-id")]
    public async Task<IActionResult> GetRaceByIdAsync(int raceId)
    {
        var races = await _raceCardRepository.GetRaceByIdAsync(raceId);
        return Ok(races);
    }


    [HttpPatch("{raceId}/status")]
    public async Task<IActionResult> UpdateStatus(int raceId, [FromBody] string status)
    {
        var allowed = new[] { "Upcoming", "Live", "Completed", "Cancelled" };
        if (!allowed.Contains(status)) return BadRequest("Invalid status");

        await _raceRepository.UpdateStatusAsync(raceId, status);

        await _hubContext.Clients.All
            .SendAsync("RaceStatusChanged", new { raceId, status });

        return Ok(new { status });
    }



}