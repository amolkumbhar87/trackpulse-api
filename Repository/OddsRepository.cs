// DAPPER — OddsRepository.cs (high frequency updates)
using Dapper;

public class OddsRepository
{
    private readonly DapperContext _dapper;
    public OddsRepository(DapperContext dapper) => _dapper = dapper;

    public async Task UpdateOddsAsync(Odds dto, int adminId)
    {
        using var conn = _dapper.CreateConnection();

        const string sql = @"
            INSERT INTO odds (race_horse_id, win_odds, place_odds, updated_at, updated_by)
            VALUES (@RaceHorseId, @WinOdds, @PlaceOdds, NOW(), @AdminId)";

        await conn.ExecuteAsync(sql, new
        {
            dto.RaceHorseId,
            dto.WinOdds,
            dto.PlaceOdds,
            AdminId = adminId
        });
    }

    // Bulk update all horses in a race at once
    public async Task BulkUpdateOddsAsync(List<Odds> updates, int adminId)
    {
        using var conn = _dapper.CreateConnection();

        const string sql = @"
            INSERT INTO odds (race_horse_id, win_odds, place_odds, updated_at, updated_by)
            VALUES (@RaceHorseId, @WinOdds, @PlaceOdds, NOW(), @AdminId)";

        await conn.ExecuteAsync(sql, updates.Select(u => new
        {
            u.RaceHorseId,
            u.WinOdds,
            u.PlaceOdds,
            AdminId = adminId
        }));
    }
}