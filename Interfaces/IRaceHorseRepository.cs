public interface IRaceHorseRepository
{
    
    Task<bool> ExistsAsync(int raceDayId, int horseId);

    Task CreateAsync(RaceHorse raceHorse);
}