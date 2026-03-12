using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
public class RaceRepository : IRaceRepository
{
    private readonly AppDbContext _db;
    public RaceRepository(AppDbContext db) => _db = db;

    

    public async Task<Race> GetByRaceDayAndNameAsync(int raceDayId, string raceName)
    {
        return await _db.Races
            .FirstOrDefaultAsync(r => r.RaceDayId == raceDayId && r.RaceName == raceName);
    }
    
    public async Task<Race> CreateAsync(Race race)
    {
        var existingRace = await _db.Races
            .FirstOrDefaultAsync(r => r.RaceDayId == race.RaceDayId && r.RaceName == race.RaceName);

        if (existingRace != null) return existingRace;

        race = new Race
        {
            RaceDayId = race.RaceDayId,
            RaceName = race.RaceName
        };

        _db.Races.Add(race);
        await _db.SaveChangesAsync();
        return race;
    }

}