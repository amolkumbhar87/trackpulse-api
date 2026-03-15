using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Dapper;
public class RaceRepository : IRaceRepository
{
    private readonly AppDbContext _db;
    private readonly DapperContext _dapper;
    public RaceRepository(AppDbContext db, DapperContext dapper)
    {
        _db = db;
        _dapper = dapper;
    }

    public async Task<Race> GetByRaceDayAndNameAsync(int raceDayId, string raceName)
    {
        return await _db.Races
            .FirstOrDefaultAsync(r => r.RaceDayId == raceDayId && r.RaceName == raceName);
    }

    public async Task<IEnumerable<dynamic>> GetRaceByCityAndDateAsync(string cityName, string raceDate)
    {
        using var conn = _dapper.CreateConnection();

        const string sql = @"SELECT 
                                r.*, 
                                v.venue_name, 
                                c.city_name, 
                                rd.race_date
                            FROM race_day rd
                            JOIN venue v 
                                ON v.venue_id = rd.venue_id
                            JOIN city c 
                                ON v.city_id = c.city_id
                            JOIN race r 
                                ON rd.race_day_id = r.race_day_id
                                    WHERE c.city_name = @CityName
                                    ";
                                    // WHERE c.city_name = @CityName  AND rd.race_date = @RaceDate

        //var races = await conn.QueryAsync(sql, new { CityName = cityName});
        var races = await conn.QueryAsync(sql, new { CityName = cityName});

        return races;
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