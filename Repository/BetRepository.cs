// EF — BetRepository.cs
using Dapper;
using Microsoft.EntityFrameworkCore;

public class BetRepository:IBetRepository
{
    private readonly AppDbContext _db;

    private readonly DapperContext _dapper;

    public BetRepository(AppDbContext db, DapperContext dapper) 
    { _db = db;
    _dapper = dapper;
    }

     public async Task<IEnumerable<BetHistoryDto>> GetBetsByUserIdAsync(int userId, string date)
    {
        using var conn = _dapper.CreateConnection();
        const string sql = @"
            SELECT 
                b.bet_id AS BetId,
                b.bet_type AS BetType,
                b.amount AS Stake,
                b.odds_at_bet AS OddsAtBet,
                b.status AS Status,
                b.placed_at AS PlacedAt,
                h.horse_name AS HorseName,
                r.race_name AS RaceName,
                rh.position AS RaceHorseNumber
            FROM trackpulse.bets b
            INNER JOIN trackpulse.race_horses rh ON b.race_horse_id = rh.race_horse_id
            INNER JOIN trackpulse.horses h ON rh.horse_id = h.horse_id
            INNER JOIN trackpulse.races r ON rh.race_id = r.race_id
            WHERE b.user_id = @UserId AND DATE(b.placed_at) = @Date ::date
            ORDER BY b.placed_at DESC";
        

        return await conn.QueryAsync<BetHistoryDto>(sql, new { UserId = userId, Date = date });
    }


    public async Task PlaceBetsAsync(BetDto betDto)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var bet = new Bet
            {
                UserId      = betDto.UserId==0 ? 1 : betDto.UserId, // Default to 1 for testing
                RaceHorseId = betDto.RaceHorseId,
                BetType     = betDto.BetType,
                Amount      = betDto.Stake,
                OddsAtBet   = betDto.Odds,
                Status      = "Pending"
            };
            _db.Bets.Add(bet);
            await _db.SaveChangesAsync();

            var txn = new BetTransaction
            {
                BetId           = bet.BetId,
                UserId          = betDto.UserId==0 ? 1 : betDto.UserId, // Default to 1 for testing
                TransactionType = "Debit",
                Amount          = betDto.Stake,
                PaymentStatus   = "Success"
            };
            _db.BetTransactions.Add(txn);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // EF — settle bets after result (bulk, Dapper faster here)
    public async Task SettleBetsAsync(int raceId, List<RaceResult> results)
    {
        var winnerHorseId = results
            .First(r => r.FinishPosition == 1).RaceHorseId;

        var placeHorseIds = results
            .Where(r => r.FinishPosition <= 3)
            .Select(r => r.RaceHorseId)
            .ToList();

        var pendingBets = await _db.Bets
            .Include(b => b.RaceHorse)
            .Where(b => b.RaceHorse.RaceId == raceId && b.Status == "Pending")
            .ToListAsync();

        foreach (var bet in pendingBets)
        {
            bet.Status = bet.BetType switch
            {
                "WIN"   => bet.RaceHorseId == winnerHorseId ? "Won" : "Lost",
                "PLACE" => placeHorseIds.Contains(bet.RaceHorseId) ? "Won" : "Lost",
                _       => "Lost"
            };

            if (bet.Status == "Won")
            {
                _db.BetTransactions.Add(new BetTransaction
                {
                    BetId           = bet.BetId,
                    UserId          = bet.UserId,
                    TransactionType = "Credit",
                    Amount          = bet.Amount * bet.OddsAtBet,
                    PaymentStatus   = "Success"
                });
            }
        }

        await _db.SaveChangesAsync();
    }

// In your betting controller/service
public async Task<bool> CanPlaceBet(int userId, int raceId)
{
    var existingBetsCount = await _db.Bets
        .Include(b => b.RaceHorse)
        .Where(b => b.UserId == userId && b.RaceHorse.RaceId == raceId)
        .CountAsync();
    
    if (existingBetsCount >= 3)
    {
        throw new InvalidOperationException("Maximum 3 bets allowed per race");
    }
    
    return true;
}
}