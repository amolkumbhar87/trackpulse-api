-- USERS
CREATE TABLE IF NOT EXISTS trackpulse.users (
    user_id SERIAL PRIMARY KEY,
    user_name VARCHAR(100) NOT NULL,
    email VARCHAR(150) NOT NULL,
    password_hash TEXT NOT NULL,
    role VARCHAR(50) NOT NULL,
    is_active BOOLEAN NOT NULL,
    session_token TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP,
    mobile_number VARCHAR(20),
    status VARCHAR(50) NOT NULL
);

-- CITY
CREATE TABLE IF NOT EXISTS trackpulse.cities (
    city_id SERIAL PRIMARY KEY,
    city_name VARCHAR(100) NOT NULL,
    state VARCHAR(100),
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- VENUE
CREATE TABLE IF NOT EXISTS trackpulse.venues (
    venue_id SERIAL PRIMARY KEY,
    city_id INT NOT NULL,
    venue_name VARCHAR(150) NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- RACE DAY
CREATE TABLE IF NOT EXISTS trackpulse.race_days (
    race_day_id SERIAL PRIMARY KEY,
    venue_id INT NOT NULL,
    race_date TIMESTAMP,
    status VARCHAR(50) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    city_name VARCHAR(100)
);

-- RACE
CREATE TABLE IF NOT EXISTS trackpulse.races (
    race_id SERIAL PRIMARY KEY,
    race_day_id INT NOT NULL,
    race_number INT NOT NULL,
    race_name VARCHAR(150) NOT NULL,
    race_type VARCHAR(50),
    distance_meters INT,
    start_time TIMESTAMP,
    status VARCHAR(50) NOT NULL,
    FOREIGN KEY (race_day_id) REFERENCES trackpulse.race_days(race_day_id)
);

-- HORSE
CREATE TABLE IF NOT EXISTS trackpulse.horses (
    horse_id SERIAL PRIMARY KEY,
    horse_name VARCHAR(150) NOT NULL,
    age INT,
    gender VARCHAR(10),
    color VARCHAR(50),
    sire VARCHAR(100),
    dam VARCHAR(100),
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- JOCKEY
CREATE TABLE IF NOT EXISTS trackpulse.jockeys (
    jockey_id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    license_no VARCHAR(100),
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- RACE HORSE
CREATE TABLE IF NOT EXISTS trackpulse.race_horses (
    race_horse_id SERIAL PRIMARY KEY,
    race_id INT NOT NULL,
    horse_id INT NOT NULL,
    jockey_id INT,
    draw_number INT NOT NULL,
    position INT NOT NULL,
    weight NUMERIC(10,2),
    rating INT,
    FOREIGN KEY (race_id) REFERENCES trackpulse.races(race_id),
    FOREIGN KEY (horse_id) REFERENCES trackpulse.horses(horse_id),
    FOREIGN KEY (jockey_id) REFERENCES trackpulse.jockeys(jockey_id)
);

-- ODDS
CREATE TABLE IF NOT EXISTS trackpulse.odds (
    odds_id SERIAL PRIMARY KEY,
    race_horse_id INT NOT NULL,
    win_odds NUMERIC(10,2),
    place_odds NUMERIC(10,2),
    updated_at TIMESTAMP NOT NULL,
    updated_by INT NOT NULL,
    FOREIGN KEY (race_horse_id) REFERENCES trackpulse.race_horses(race_horse_id)
);

-- BET
CREATE TABLE IF NOT EXISTS trackpulse.bets (
    bet_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    race_horse_id INT NOT NULL,
    bet_type VARCHAR(50) NOT NULL,
    amount NUMERIC(18,2) NOT NULL,
    odds_at_bet NUMERIC(10,2) NOT NULL,
    status VARCHAR(50) NOT NULL,
    placed_at TIMESTAMP NOT NULL,
    FOREIGN KEY (user_id) REFERENCES trackpulse.users(user_id),
    FOREIGN KEY (race_horse_id) REFERENCES trackpulse.race_horses(race_horse_id)
);

-- BET TRANSACTION
CREATE TABLE IF NOT EXISTS trackpulse.bet_transactions (
    transaction_id SERIAL PRIMARY KEY,
    bet_id INT NOT NULL,
    user_id INT NOT NULL,
    transaction_type VARCHAR(50) NOT NULL,
    amount NUMERIC(18,2) NOT NULL,
    payment_status VARCHAR(50) NOT NULL,
    reference_no VARCHAR(100),
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (bet_id) REFERENCES trackpulse.bets(bet_id),
    FOREIGN KEY (user_id) REFERENCES trackpulse.users(user_id)
);

-- RACE RESULT
CREATE TABLE IF NOT EXISTS trackpulse.race_results (
    result_id SERIAL PRIMARY KEY,
    race_id INT NOT NULL,
    race_horse_id INT NOT NULL,
    finish_position INT NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (race_id) REFERENCES trackpulse.races(race_id),
    FOREIGN KEY (race_horse_id) REFERENCES trackpulse.race_horses(race_horse_id)
);