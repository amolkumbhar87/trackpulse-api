using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Dapper;
public class RaceDayRepository : IRaceDayRepository
{
    private readonly AppDbContext _db;
    private readonly DapperContext _dapper;
    public RaceDayRepository(AppDbContext db, DapperContext dapper)
    {
        _db = db;
        _dapper = dapper;
    }





    public async Task<RaceDay> CreateAsync(RaceDay raceDay)
    {

        if (raceDay.RaceDate.HasValue)
        {
            raceDay.RaceDate = DateTime.SpecifyKind(raceDay.RaceDate.Value.Date, DateTimeKind.Utc);
        }

        var existingRaceDay = await _db.RaceDays
            .FirstOrDefaultAsync(rd => rd.RaceDate == raceDay.RaceDate && rd.CityName == raceDay.CityName);

        if (existingRaceDay != null) return existingRaceDay;

        _db.RaceDays.Add(raceDay);
        await _db.SaveChangesAsync();
        return raceDay;
    }
}