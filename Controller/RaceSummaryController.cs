using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
public class RaceSummaryController : ControllerBase
{
    private readonly IRaceSummaryRepository _raceSummaryRepository;
    public RaceSummaryController(IRaceSummaryRepository repo) => _raceSummaryRepository = repo;

    [HttpGet("day")]
    public async Task<IActionResult> Day([FromQuery] DateTime date)
        => Ok(await _raceSummaryRepository.GetDaySummaryAsync(date));

    [HttpGet("race/{raceId}")]
    public async Task<IActionResult> Race(int raceId)
        => Ok(await _raceSummaryRepository.GetRaceSummaryAsync(raceId));

    [HttpGet("race/{raceId}/horse/{raceHorseId}")]
    public async Task<IActionResult> Horse(int raceId, int raceHorseId)
        => Ok(await _raceSummaryRepository.GetHorseUsersAsync(raceId, raceHorseId));
}