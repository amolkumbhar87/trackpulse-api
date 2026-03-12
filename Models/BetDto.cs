public class BetDto
{
    public int UserId { get; set; }
    public int RaceHorseId { get; set; }
    public decimal Amount { get; set; }
    public string BetType { get; set; } // "Win" or "Place"
}