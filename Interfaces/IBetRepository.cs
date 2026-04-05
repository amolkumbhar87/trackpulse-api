
public interface IBetRepository
{
    Task PlaceBetsAsync(BetDto betDto);

    Task<IEnumerable<BetHistoryDto>> GetBetsByUserIdAsync(int userId, string date);
    Task<bool> CanPlaceBet(int userId, int raceId);
}