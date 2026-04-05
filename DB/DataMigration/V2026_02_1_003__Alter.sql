-- Drop constraint if it exists (ignore error if it doesn't)
ALTER TABLE trackpulse.odds DROP CONSTRAINT IF EXISTS unique_race_horse_id;

-- Add the constraint
ALTER TABLE trackpulse.odds ADD CONSTRAINT unique_race_horse_id UNIQUE (race_horse_id);

-- Add column (keep IF NOT EXISTS as it's supported)
ALTER TABLE trackpulse.users ADD COLUMN IF NOT EXISTS wallet_balance DECIMAL(12,2) NOT NULL DEFAULT 0.00;