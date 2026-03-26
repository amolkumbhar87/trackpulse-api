public interface IRaceRepository
{
    
    Task<Race> GetByRaceDayAndNameAsync(int raceDayId, string raceName);

    Task<Race> CreateAsync(Race race);

    Task<IEnumerable<dynamic>> GetRaceByCityAndDateAsync(string cityName, string raceDate);

}