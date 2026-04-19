CREATE OR REPLACE FUNCTION trackpulse.get_horse_bet_summary(p_race_id INT)
RETURNS TABLE (
    race_name           VARCHAR,
    race_horse_id   INT,
    horse_name          VARCHAR,
    draw_number         INT,
    win_stake           NUMERIC,
    win_bet_count       BIGINT,
    win_payout          NUMERIC,
    place_stake         NUMERIC,
    place_bet_count     BIGINT,
    place_payout        NUMERIC,
    total_stake         NUMERIC,
    total_bets          BIGINT,
    total_payout        NUMERIC
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT 
        r.race_name::VARCHAR,
        h.horse_name::VARCHAR,
        rh.race_horse_id,
        MAX(rh.draw_number)::INT AS draw_number,
        -- Win bets
        COALESCE(SUM(CASE WHEN b.bet_type = 'win' THEN b.amount ELSE 0 END), 0) AS win_stake,
        COUNT(CASE WHEN b.bet_type = 'win' THEN 1 END)::BIGINT AS win_bet_count,
        COALESCE(SUM(CASE WHEN b.bet_type = 'win' THEN b.amount * b.odds_at_bet ELSE 0 END), 0) AS win_payout,
        -- Place bets
        COALESCE(SUM(CASE WHEN b.bet_type = 'place' THEN b.amount ELSE 0 END), 0) AS place_stake,
        COUNT(CASE WHEN b.bet_type = 'place' THEN 1 END)::BIGINT AS place_bet_count,
        COALESCE(SUM(CASE WHEN b.bet_type = 'place' THEN b.amount * b.odds_at_bet ELSE 0 END), 0) AS place_payout,
        -- Combined
        COALESCE(SUM(b.amount), 0) AS total_stake,
        COUNT(*)::BIGINT AS total_bets,
        COALESCE(SUM(b.amount * b.odds_at_bet), 0) AS total_payout
    FROM trackpulse.bets b
    JOIN trackpulse.race_horses rh ON rh.race_horse_id = b.race_horse_id
    JOIN trackpulse.horses h ON h.horse_id = rh.horse_id
    JOIN trackpulse.races r ON r.race_id = rh.race_id
    WHERE r.race_id = p_race_id
    GROUP BY r.race_name, h.horse_name,rh.race_horse_id
    ORDER BY draw_number;
END;
$$;