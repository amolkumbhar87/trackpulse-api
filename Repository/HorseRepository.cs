using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
public class HorseRepository : IHorseRepository
{
    private readonly AppDbContext _db;
    public HorseRepository(AppDbContext db) => _db = db;

    public async Task<Horse> GetOrCreateAsync(string horseName, int? age, string? color, string? gender)
    {
        var horse = await _db.Horses
            .FirstOrDefaultAsync(h => h.HorseName == horseName);

        if (horse != null) return horse;

        horse = new Horse
        {
            HorseName = horseName,
            Age       = age,
            Color     = color,
            Gender    = gender,
            IsActive  = true
        };

        _db.Horses.Add(horse);
        await _db.SaveChangesAsync();
        return horse;
    }
}