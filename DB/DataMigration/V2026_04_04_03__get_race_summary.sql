CREATE OR REPLACE FUNCTION trackpulse.get_race_summary(p_race_id INT)
RETURNS TABLE (
    race_horse_id   INT,
    draw_number     INT,
    horse_name      VARCHAR,
    "position"        INT,
    bet_type        VARCHAR,
    bet_count       BIGINT,
    total_staked    NUMERIC,
    total_liability NUMERIC,
    result          TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT
        ars.race_horse_id,
        ars.draw_number,
        ars.horse_name,
        ars.position,
        ars.bet_type,
        COUNT(ars.bet_id)::BIGINT  AS bet_count,
        SUM(ars.bet_amount)        AS total_staked,
        SUM(ars.potential_payout)  AS total_liability,
        CASE
            WHEN ars.position = 1               THEN 'Winner'
            WHEN ars.position BETWEEN 2 AND 3   THEN 'Placed'
            WHEN ars.position IS NULL            THEN 'Pending'
            ELSE 'Unplaced'
        END AS result
    FROM trackpulse.admin_race_summary ars
    WHERE ars.race_id = p_race_id
    GROUP BY
        ars.race_horse_id, ars.draw_number, ars.horse_name,
        ars.position, ars.bet_type
    ORDER BY ars.draw_number;
END;
$$;