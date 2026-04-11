CREATE INDEX IF NOT EXISTS  idx_admin_race_summary_race_date ON trackpulse.admin_race_summary (race_date);
CREATE INDEX IF NOT EXISTS idx_admin_race_summary_race_id ON trackpulse.admin_race_summary (race_id);
CREATE INDEX IF NOT EXISTS idx_admin_race_summary_user_id ON trackpulse.admin_race_summary (user_id);
CREATE INDEX IF NOT EXISTS idx_admin_race_summary_race_status ON trackpulse.admin_race_summary (race_status);

CREATE UNIQUE INDEX IF NOT EXISTS idx_admin_race_summary_bet_id ON trackpulse.admin_race_summary (bet_id);