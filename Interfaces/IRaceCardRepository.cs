public interface IRaceCardRepository
{
    Task<IEnumerable<RaceCardDto>> GetTodaysRaceCardAsync();
}   