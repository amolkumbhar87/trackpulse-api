CREATE MATERIALIZED VIEW trackpulse.admin_race_summary AS
SELECT
    rd.race_date,
    rd.city_name,
    rd.race_day_id,

    r.race_id,
    r.race_number,
    r.race_name,
    r.start_time,
    r.status                            AS race_status,

    rh.race_horse_id,
    rh.draw_number,
    rh.position,                      
    rh.weight,
    rh.rating,

    h.horse_name                         ,  

    b.bet_type,
    b.odds_at_bet,

    u.user_id,
    u.user_name,
    u.mobile_number,

    b.bet_id,
    b.amount                            AS bet_amount,
    b.status                            AS bet_status,
    b.placed_at,

    ROUND(b.amount * b.odds_at_bet, 2)  AS potential_payout,

    COALESCE(dep.approved_deposit, 0)   AS user_approved_deposit

FROM trackpulse.bets b
JOIN trackpulse.race_horses rh  ON rh.race_horse_id = b.race_horse_id
JOIN trackpulse.horses h        ON h.horse_id             = rh.horse_id    -- ✅ new join
JOIN trackpulse.races r         ON r.race_id         = rh.race_id
JOIN trackpulse.race_days rd    ON rd.race_day_id    = r.race_day_id
JOIN trackpulse.users u         ON u.user_id          = b.user_id
LEFT JOIN (
    SELECT user_id, DATE(reviewed_at) AS deposit_date, SUM(amount) AS approved_deposit
    FROM trackpulse.deposit_requests
    WHERE status = 'approved'
    GROUP BY user_id, DATE(reviewed_at)
) dep ON dep.user_id = u.user_id AND dep.deposit_date = rd.race_date

WITH DATA;