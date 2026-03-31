CREATE INDEX idx_bets_user_id ON trackpulse.bets(user_id);
CREATE INDEX idx_bets_race_horse_id ON trackpulse.bets(race_horse_id);

CREATE INDEX idx_transactions_bet_id ON trackpulse.bet_transactions(bet_id);

CREATE INDEX idx_race_horses_race_id ON trackpulse.race_horses(race_id);

CREATE INDEX idx_odds_race_horse_id ON trackpulse.odds(race_horse_id);

CREATE INDEX idx_races_race_day_id ON trackpulse.races(race_day_id);

CREATE UNIQUE INDEX idx_city_state ON trackpulse.cities (city_name, state);

CREATE UNIQUE INDEX idx_city_venue ON trackpulse.venues (city_id, venue_name);