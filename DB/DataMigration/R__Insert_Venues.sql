INSERT INTO trackpulse.venues (city_id, venue_name, is_active) VALUES
    (1, 'Mahalaxmi Racecourse', TRUE),
    (2, 'Royal Calcutta Turf Club', TRUE),
    (3, 'Delhi Race Club', TRUE),
    (4, 'Bangalore Turf Club', TRUE),
    (5, 'Hyderabad Race Club', TRUE),
    (6, 'Mysore Race Club', TRUE),
    (7, 'Madras Race Club', TRUE),    
    (8, 'Pune Racecourse', TRUE)  
     
ON CONFLICT (city_id, venue_name) DO NOTHING;