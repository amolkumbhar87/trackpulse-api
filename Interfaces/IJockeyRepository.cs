public interface IJockeyRepository
{
    Task<Jockey> GetOrCreateAsync(string name);
}