using Dapper;

public class RaceSummaryRepository : IRaceSummaryRepository
{
    private readonly DapperContext _ctx;
    public RaceSummaryRepository(DapperContext ctx) => _ctx = ctx;

    // Day summary
    public async Task<IEnumerable<DaySummaryDto>> GetDaySummaryAsync(DateTime datevalue)
    {
        var dateString = datevalue.ToString("yyyy-MM-dd");

    
    const string sql = @"SELECT  race_id as RaceId, 
        race_name as RaceName, race_number as RaceNumber, 
        start_time as StartTime, race_status as RaceStatus, 
        city_name as CityName, unique_bettors as UniqueBettors, 
        total_bets as TotalBets, total_staked as TotalStaked, 
        total_liability as TotalLiability, win_staked as WinStaked, place_staked as PlaceStaked
    
       FROM trackpulse.get_day_summary(@Date ::date)";
    using var conn = _ctx.CreateConnection();
    var result = await conn.QueryAsync<DaySummaryDto>(sql, new { Date = dateString });
    return result;
    }

    // Race drill-down
    public async Task<IEnumerable<RaceSummaryDto>> GetRaceSummaryAsync(int raceId)
    {
        const string sql = @"SELECT race_horse_id as RaceHorseId, 
                            horse_name as HorseName, 
                            draw_number as DrawNumber,
                            Position, 
                            bet_type as BetType, bet_count as  BetCount, 
                            total_staked as TotalStaked, total_liability as TotalLiability, result as Result
        
         FROM trackpulse.get_race_summary(@RaceId)";
        using var conn = _ctx.CreateConnection();
        return await conn.QueryAsync<RaceSummaryDto>(sql, new { RaceId = raceId });
    }

    // Horse user drill-down
    public async Task<IEnumerable<HorseUserDto>> GetHorseUsersAsync(int raceId, int raceHorseId)
    {
        const string sql = @"SELECT  
                user_id as UserId, user_name as UserName, mobile_number as MobileNumber, 
                                bet_type as BetType, bet_amount as BetAmount, 
                                odds_at_bet as OddsAtBet, potential_payout as PotentialPayout, 
                                bet_status as BetStatus, placed_at as PlacedAt
        
             FROM trackpulse.get_horse_users(@RaceId, @RaceHorseId)";
        using var conn = _ctx.CreateConnection();
        var result = await conn.QueryAsync<HorseUserDto>(sql, new { RaceId = raceId, RaceHorseId = raceHorseId });
        return result;
    }
}