
public interface IBetRepository
{
    Task<Bet> GetOrCreateAsync(int horseId, int betAmount, string bettorName);
}