using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/[controller]")]
public class BetController : ControllerBase
{


    private readonly IRaceRepository _raceRepository;
    private readonly IHorseRepository _horseRepository;
    private readonly IRaceHorseRepository _raceHorseRepository;

    private readonly IRaceCardRepository _raceCardRepository;

    private readonly IHubContext<OddsHub> _hubContext;

    private readonly IBetRepository _betRepository;

    private readonly IUserRepository _userRepository;

    public BetController(IRaceRepository raceRepository, IHorseRepository horseRepository,
    IRaceHorseRepository raceHorseRepository,
    IRaceCardRepository raceCardRepository,
    IHubContext<OddsHub> hubContext,
    IBetRepository betRepository,
    IUserRepository userRepository
    )
    {
        _raceRepository = raceRepository;
        _horseRepository = horseRepository;
        _raceHorseRepository = raceHorseRepository;
        _raceCardRepository = raceCardRepository;
        _hubContext = hubContext;
        _betRepository = betRepository;
        _userRepository = userRepository;

    }


    [HttpPost("bet-by-horse")]
    public async Task<IActionResult> PlaceBetsAsync(BetDto betDto)
    {
        var userId = 2; //int.Parse(User.FindFirst("sub")!.Value);
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
    public async Task<IActionResult> GetUserBetsCountForRace([FromQuery] int raceId)
    {
        int userId = 2; // Replace with actual user ID retrieval logic
        var canPlace = await _betRepository.CanPlaceBet(userId, raceId);
        return Ok(canPlace);
    }


}