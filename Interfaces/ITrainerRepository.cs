public interface ITrainerRepository
{
    Task<Trainer> GetOrCreateByNameAsync(string trainerName);
    
}