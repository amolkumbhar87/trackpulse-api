public interface IRaceCardRepository
{
    Task<IEnumerable<RaceCardDto>> GetTodaysRaceCardAsync();

    Task<IEnumerable<Races>> GetRaceByDateAsync(string raceDate);

    Task<IEnumerable<RaceCardDto>> GetRaceByIdAsync(int raceId);

}   