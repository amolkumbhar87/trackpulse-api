CREATE OR REPLACE FUNCTION trackpulse.get_horse_users(p_race_id INT, p_race_horse_id INT)
RETURNS TABLE (
    user_id          INT,
    user_name        VARCHAR,
    mobile_number    VARCHAR,
    bet_type         VARCHAR,
    bet_amount       NUMERIC,
    odds_at_bet      NUMERIC,
    potential_payout NUMERIC,
    bet_status       VARCHAR,
    placed_at        TIMESTAMP
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        ars.user_id,
        ars.user_name,
        ars.mobile_number,
        ars.bet_type,
        ars.bet_amount,
        ars.odds_at_bet,
        ars.potential_payout,
        ars.bet_status,
        ars.placed_at
    FROM trackpulse.admin_race_summary ars
    WHERE ars.race_id       = p_race_id
      AND ars.race_horse_id = p_race_horse_id
    ORDER BY ars.placed_at;
END;
$$;