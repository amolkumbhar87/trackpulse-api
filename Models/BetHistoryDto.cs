public class BetHistoryDto
{
    public int BetId { get; set; }
    public string BetType { get; set; }
    public decimal Stake { get; set; }
    public decimal OddsAtBet { get; set; }
    public string Status { get; set; }
    public DateTime PlacedAt { get; set; }
    public string HorseName { get; set; }
    public string RaceName { get; set; }
    public DateTime RaceDate { get; set; }
    public int? RaceHorseNumber { get; set; }
}