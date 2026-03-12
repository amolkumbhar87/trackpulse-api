using Microsoft.EntityFrameworkCore;

// EF — JockeyRepository.cs
public class JockeyRepository
{
    private readonly AppDbContext _db;
    public JockeyRepository(AppDbContext db) => _db = db;

    public async Task<Jockey> GetOrCreateAsync(string name)
    {
        var jockey = await _db.Jockeys
            .FirstOrDefaultAsync(j => j.Name == name && j.IsActive);

        if (jockey != null) return jockey;

        jockey = new Jockey { Name = name, IsActive = true };
        _db.Jockeys.Add(jockey);
        await _db.SaveChangesAsync();
        return jockey;
    }
}