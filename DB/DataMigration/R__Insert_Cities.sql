INSERT INTO trackpulse.cities (city_name, state, is_active) VALUES
('MUMBAI', 'Maharashtra', TRUE),
('KOLKATA', 'West Bengal', TRUE),
('DELHI', 'Delhi', TRUE),
('BANGALORE', 'Karnataka', TRUE),
('HYDERABAD', 'Telangana', TRUE),
('MYSORE', 'Karnataka', TRUE),
('Chennai',   'Tamil Nadu', TRUE),
('PUNE', 'Maharashtra', TRUE)
ON CONFLICT (city_name, state) DO NOTHING;