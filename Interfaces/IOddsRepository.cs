public interface IOddsRepository
{
    public Task UpdateOddsAsync(Odds dto, int adminId);
   public Task BulkUpdateOddsAsync(List<Odds> horseOdds);
}