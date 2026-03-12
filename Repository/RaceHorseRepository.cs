
using Microsoft.EntityFrameworkCore;

public class RaceHorseRepository : IRaceHorseRepository
{
    private readonly AppDbContext _db;

    public RaceHorseRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task CreateAsync(RaceHorse raceHorse)
    {
        var existingRaceHorse = await _db.RaceHorses
            .FirstOrDefaultAsync(rh => rh.RaceId == raceHorse.RaceId && rh.HorseId == raceHorse.HorseId);

        if (existingRaceHorse != null) return;

        _db.RaceHorses.Add(raceHorse);
        await _db.SaveChangesAsync();
        
    }

    

    public async Task<bool> ExistsAsync(int raceId, int horseId)
{
    return await _db.RaceHorses
        .AnyAsync(rh => rh.RaceId == raceId && rh.HorseId == horseId);
}
}