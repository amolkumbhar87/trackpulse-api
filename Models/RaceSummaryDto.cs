public class RaceSummaryDto
{
    public int     RaceHorseId   { get; set; }
    public int     DrawNumber    { get; set; }
    public string  HorseName     { get; set; } = "";
    public int?    Position      { get; set; }
    public string  BetType       { get; set; } = "";
    public long    BetCount      { get; set; }
    public decimal TotalStaked   { get; set; }
    public decimal TotalLiability{ get; set; }
    public string  Result        { get; set; } = "";
}