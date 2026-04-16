using System.Text.Json;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Dapper;

[ApiController]
[Route("api/[controller]")]
public class RaceResultController : ControllerBase
{

    private readonly IHubContext<OddsHub> _hubContext;
    private readonly DapperContext _dapperContext;

    public RaceResultController(IHubContext<OddsHub> hubContext, DapperContext dapperContext)
    {
        _hubContext = hubContext;
        _dapperContext = dapperContext;
    }

    // Controller
    [HttpPost("result")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SubmitResult(RaceResultDto dto)
    {
        var adminId = int.Parse(User.FindFirst("sub")!.Value);
        var positions = JsonSerializer.Serialize(dto.Positions.Select(p => new
    {
        raceHorseId = p.RaceHorseId,
        position = p.Position
    }));
    
    // 1. Run the stored procedure — get bet outcomes
    const string sql = """
        SELECT * FROM trackpulse.process_race_result(@RaceId, @Positions::jsonb, @AdminId)
        """;

        using var conn = _dapperContext.CreateConnection();
        var results = (await conn.QueryAsync(sql, new
        {
            RaceId = dto.RaceId,
            Positions = positions,
            AdminId = adminId
        })).ToList();

        // 2. Broadcast race result to all users via SignalR
        await _hubContext.Clients.All.SendAsync("RaceResultPublished", new
        {
            raceId = dto.RaceId,
            results = results.Select(r => new
            {
                userId = r.user_id,
                betId = r.bet_id,
                status = r.status,
                payout = r.payout
            })
        });

        // 3. Send personal notification to each affected user
        foreach (var r in results)
        {
            await _hubContext.Clients
                .User(r.user_id.ToString())
                .SendAsync("BetSettled", new
                {
                    betId = r.bet_id,
                    status = r.status,
                    payout = r.payout,
                    betType = r.bet_type
                });
        }

        return Ok(new { settled = results.Count });
    }

    // Get notifications for logged-in user
    [HttpGet("notifications")]
    [Authorize]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = int.Parse(User.FindFirst("sub")!.Value);
        const string sql = """
        SELECT * FROM trackpulse.notifications
        WHERE user_id = @UserId
        ORDER BY created_at DESC
        LIMIT 50
        """;
        using var conn = _dapperContext.CreateConnection();
        var notifs = await conn.QueryAsync(sql, new { UserId = userId });
        return Ok(notifs);
    }

    // Mark notifications read
    [HttpPatch("notifications/read")]
    [Authorize]
    public async Task<IActionResult> MarkRead()
    {
        var userId = int.Parse(User.FindFirst("sub")!.Value);
        const string sql = """
        UPDATE trackpulse.notifications
        SET is_read = TRUE
        WHERE user_id = @UserId AND is_read = FALSE
        """;
        using var conn = _dapperContext.CreateConnection();
        await conn.ExecuteAsync(sql, new { UserId = userId });
        return Ok();
    }
}