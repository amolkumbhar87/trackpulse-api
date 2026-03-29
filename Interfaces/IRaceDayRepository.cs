public interface IRaceDayRepository
{
    Task<RaceDay> CreateAsync(RaceDay race);
}