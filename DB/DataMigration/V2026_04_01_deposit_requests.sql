CREATE TABLE deposit_requests (
    id                SERIAL PRIMARY KEY,
    user_id           INT NOT NULL REFERENCES users(id),
    amount            DECIMAL(12,2) NOT NULL CHECK (amount > 0),
    payment_method    VARCHAR(20) NOT NULL DEFAULT 'upi',
    transaction_id    VARCHAR(100) NOT NULL,
    notes             VARCHAR(500),
    screenshot_path   VARCHAR(500),          -- relative path or S3/Cloudinary URL
    status            VARCHAR(20) NOT NULL DEFAULT 'pending'
                          CHECK (status IN ('pending','approved','rejected')),
    rejection_reason  VARCHAR(200),
    reviewed_by       INT REFERENCES users(id),   -- admin user id
    submitted_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    reviewed_at       TIMESTAMPTZ
);