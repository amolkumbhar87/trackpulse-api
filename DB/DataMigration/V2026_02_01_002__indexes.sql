CREATE INDEX IF NOT EXISTS idx_bets_user_id ON trackpulse.bets(user_id);
CREATE INDEX IF NOT EXISTS idx_bets_race_horse_id ON trackpulse.bets(race_horse_id);

CREATE INDEX IF NOT EXISTS idx_transactions_bet_id ON trackpulse.bet_transactions(bet_id);

CREATE INDEX IF NOT EXISTS idx_race_horses_race_id ON trackpulse.race_horses(race_id);

CREATE INDEX IF NOT EXISTS idx_odds_race_horse_id ON trackpulse.odds(race_horse_id);

CREATE INDEX IF NOT EXISTS idx_races_race_day_id ON trackpulse.races(race_day_id);

CREATE UNIQUE INDEX IF NOT EXISTS idx_city_state ON trackpulse.cities (city_name, state);

CREATE UNIQUE INDEX IF NOT EXISTS idx_city_venue ON trackpulse.venues (city_id, venue_name);

CREATE INDEX IF NOT EXISTS idx_deposit_status      ON trackpulse.deposit_requests(status);

CREATE INDEX IF NOT EXISTS idx_deposit_user        ON trackpulse.deposit_requests(user_id);

CREATE INDEX IF NOT EXISTS idx_deposit_submitted   ON trackpulse.deposit_requests(submitted_at DESC);