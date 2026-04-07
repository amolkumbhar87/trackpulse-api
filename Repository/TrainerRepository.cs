using Dapper;
using Microsoft.EntityFrameworkCore;
public class TrainerRepository : ITrainerRepository
{
    private readonly AppDbContext _db;
    public TrainerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Trainer> GetOrCreateByNameAsync(string trainerName)
    {
        var existingTrainer = await _db.Trainers
            .FirstOrDefaultAsync(t => t.Name == trainerName);

        if (existingTrainer != null) return existingTrainer;

        var trainer = new Trainer { Name = trainerName };
        _db.Trainers.Add(trainer);
        await _db.SaveChangesAsync();
        return trainer;
    }
}