public class BetDto
{

public int RaceHorseId { get; set; }

public int RaceId { get; set; }
    public int UserId { get; set; }
    public decimal Stake { get; set; }
    public string BetType { get; set; } // "Win" or "Place"
    public decimal Odds { get; set; } // e.g. "5/1" or "2.5"
}