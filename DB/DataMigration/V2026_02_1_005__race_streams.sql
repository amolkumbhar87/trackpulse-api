CREATE TABLE  IF NOT EXISTS trackpulse.race_streams (
    id SERIAL PRIMARY KEY,
    race_id INT  NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    stream_url VARCHAR(500) NOT NULL,
    status VARCHAR(50) NOT NULL, -- 'live' | 'upcoming' | 'recorded'
    venue VARCHAR(255),
    race_date TIMESTAMPTZ,
    race_name VARCHAR(255),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    is_active BOOLEAN NOT NULL DEFAULT TRUE
)