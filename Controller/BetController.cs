using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/[controller]")]
public class BetController : ControllerBase
{


    private readonly IBetRepository _betRepository;

    private readonly IUserRepository _userRepository;

    public BetController(
    IBetRepository betRepository,
    IUserRepository userRepository
    )
    {

        _betRepository = betRepository;
        _userRepository = userRepository;

    }


    [HttpPost("bet-by-horse")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> PlaceBetsAsync(BetDto betDto)
    {
        var userId = int.Parse(User.FindFirst("uid")!.Value);
        betDto.UserId = userId;

        var balance = await _userRepository.GetWalletBalanceAsync(userId);
        if (balance < betDto.Stake)
        {
            return BadRequest(new { error = "Insufficient wallet balance." });
        }

        // 2. Deduct wallet
        await _userRepository.DeductAsync(userId, betDto.Stake);

        await _betRepository.PlaceBetsAsync(betDto);
        return Ok();
    }

    [HttpGet("history/{userId}/{date}")]
    public async Task<IActionResult> GetBetsByUserIdAsync(int userId, string date)
    {
        var bets = await _betRepository.GetBetsByUserIdAsync(userId, date);
        return Ok(bets);
    }

    [HttpGet("get-user-bets-count-for-race")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetUserBetsCountForRace([FromQuery] int raceId)
    {
        var userId = int.Parse(User.FindFirst("uid")!.Value);
        var canPlace = await _betRepository.CanPlaceBet(userId, raceId);
        return Ok(canPlace);
    }


}