public interface IRaceRepository
{
    
    Task<Race> GetByRaceDayAndNameAsync(int raceDayId, string raceName);

    Task<Race> CreateAsync(Race race);
}