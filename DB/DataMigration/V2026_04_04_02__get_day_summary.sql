CREATE OR REPLACE FUNCTION trackpulse.get_day_summary(p_date DATE)
RETURNS TABLE (
    race_id         INT,
    race_name       VARCHAR,
    race_number     INT,
    start_time      TIMESTAMP,
    race_status     VARCHAR,
    city_name       VARCHAR,
    unique_bettors  BIGINT,
    total_bets      BIGINT,
    total_staked    NUMERIC,
    total_liability NUMERIC,
    win_staked      NUMERIC,
    place_staked    NUMERIC
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        ars.race_id,
        ars.race_name,
        ars.race_number,
        ars.start_time,
        ars.race_status,
        ars.city_name,
        COUNT(DISTINCT ars.user_id)::BIGINT                                   AS unique_bettors,
        COUNT(ars.bet_id)::BIGINT                                             AS total_bets,
        SUM(ars.bet_amount)                                                   AS total_staked,
        SUM(ars.potential_payout)                                             AS total_liability,
        SUM(CASE WHEN ars.bet_type = 'win'   THEN ars.bet_amount ELSE 0 END) AS win_staked,
        SUM(CASE WHEN ars.bet_type = 'place' THEN ars.bet_amount ELSE 0 END) AS place_staked
    FROM trackpulse.admin_race_summary ars
    WHERE ars.race_date = p_date
    GROUP BY
        ars.race_id, ars.race_name, ars.race_number,
        ars.start_time, ars.race_status, ars.city_name
    ORDER BY ars.race_number;
END;
$$;