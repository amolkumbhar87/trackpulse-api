
public interface IBetRepository
{
    Task PlaceBetsAsync(BetDto betDto);

    Task<IEnumerable<BetHistoryDto>> GetBetsByUserIdAsync(int userId, string date);
    Task<int> GetUserBetsCountForRace(int userId, int raceId);
}