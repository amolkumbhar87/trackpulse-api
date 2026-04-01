// EF — BetRepository.cs
using Microsoft.EntityFrameworkCore;

public class BetRepository:IBetRepository
{
    private readonly AppDbContext _db;
    public BetRepository(AppDbContext db) => _db = db;

    public Task<Bet> GetOrCreateAsync(int horseId, int betAmount, string bettorName)
    {
        throw new NotImplementedException();
    }

    public async Task<Bet> PlaceBetAsync(Bet dto)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var bet = new Bet
            {
                UserId      = dto.UserId,
                RaceHorseId = dto.RaceHorseId,
                BetType     = dto.BetType,
                Amount      = dto.Amount,
                OddsAtBet   = dto.OddsAtBet,
                Status      = "Pending"
            };
            _db.Bets.Add(bet);
            await _db.SaveChangesAsync();

            var txn = new BetTransaction
            {
                BetId           = bet.BetId,
                UserId          = dto.UserId,
                TransactionType = "Debit",
                Amount          = dto.Amount,
                PaymentStatus   = "Success"
            };
            _db.BetTransactions.Add(txn);
            await _db.SaveChangesAsync();

            await transaction.CommitAsync();
            return bet;
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
}