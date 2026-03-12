// DAPPER — RaceCardRepository.cs (complex join)
using Dapper;

public class RaceCardRepository : IRaceCardRepository
{
    private readonly DapperContext _dapper;
    public RaceCardRepository(DapperContext dapper) => _dapper = dapper;

    public async Task<IEnumerable<RaceCardDto>> GetTodaysRaceCardAsync()
    {
        using var conn = _dapper.CreateConnection();

        const string sql = @"
            SELECT
                r.race_id           AS RaceId,
                r.race_number       AS RaceNumber,
                r.race_name         AS RaceName,
                r.race_type         AS RaceType,
                r.distance_meters   AS DistanceMeters,
                r.start_time        AS StartTime,
                r.status            AS Status,
                v.venue_name        AS VenueName,
                c.city_name         AS CityName,
                rh.race_horse_id    AS RaceHorseId,
                rh.draw_number      AS DrawNumber,
                rh.weight           AS Weight,
                rh.rating           AS Rating,
                h.horse_name        AS HorseName,
                h.age               AS Age,
                h.color             AS Color,
                h.gender            AS Gender,
                j.name              AS JockeyName,
                lo.win_odds         AS WinOdds,
                lo.place_odds       AS PlaceOdds
            FROM race r
            JOIN race_day rd    ON rd.race_day_id   = r.race_day_id
            JOIN venue v        ON v.venue_id        = rd.venue_id
            JOIN city c         ON c.city_id         = v.city_id
            JOIN race_horse rh  ON rh.race_id        = r.race_id
            JOIN horse h        ON h.horse_id        = rh.horse_id
            LEFT JOIN jockey j  ON j.jockey_id       = rh.jockey_id
            LEFT JOIN LATERAL (
                SELECT win_odds, place_odds
                FROM odds
                WHERE race_horse_id = rh.race_horse_id
                ORDER BY updated_at DESC
                LIMIT 1
            ) lo ON TRUE
            WHERE rd.race_date = CURRENT_DATE
            ORDER BY r.race_number, rh.draw_number";

        return await conn.QueryAsync<RaceCardDto>(sql);
    }
}