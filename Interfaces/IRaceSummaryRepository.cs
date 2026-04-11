public interface IRaceSummaryRepository
{
    Task<IEnumerable<DaySummaryDto>> GetDaySummaryAsync(DateTime date);
   Task<IEnumerable<RaceSummaryDto>> GetRaceSummaryAsync(int raceId);
    Task<IEnumerable<HorseUserDto>> GetHorseUsersAsync(int raceId, int raceHorseId);
}