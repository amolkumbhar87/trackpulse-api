
    public class ParsedRaceInfo
    {
        
        public int RaceId { get; set; }
        public int RaceNumber { get; set; }
        public string RaceName { get; set; }
        public int DistanceMeters { get; set; }
        public DateTime StartTime { get; set; }
        public string Status { get; set; }
        public List<ParsedHorseInfo> Horses { get; set; } = new List<ParsedHorseInfo>();
    }

    public class ParsedHorseInfo
    {
        public int Position { get; set; }  // Race number (1,2,3...)
        public int DrawNumber { get; set; }  // Draw number in parentheses
        public string HorseName { get; set; }
        public string Pedigree { get; set; }  // Sire-Dam
        public string Description { get; set; }  // e.g., "4y ch g"
        public int Age { get; set; }
        public string Color { get; set; }
        public string Gender { get; set; }
        public string Owner { get; set; }
        public string Trainer { get; set; }
        public string Jockey { get; set; }
        public decimal Weight { get; set; }
        public string Allowance { get; set; }  // Al column
        public string Shoe { get; set; }  // Sh column
        public string Equipment { get; set; }  // Eq column
        public int Rating { get; set; }
        public List<int> LastFiveRuns { get; set; } = new List<int>();
    }
