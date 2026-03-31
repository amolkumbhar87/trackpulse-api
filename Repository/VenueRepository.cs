    
using Microsoft.EntityFrameworkCore;
public class VenueRepository : IVenueRepository
{
    private readonly AppDbContext _db;
    public VenueRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Venue>> GetAllAsync()
    {

        return await _db.Venues.Include(v => v.City).ToListAsync();
    }

   
}