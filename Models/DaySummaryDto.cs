public class DaySummaryDto
{
    public int      RaceId         { get; set; }
    public string   RaceName       { get; set; } = "";
    public int      RaceNumber     { get; set; }
    public DateTime StartTime      { get; set; }
    public string   RaceStatus     { get; set; } = "";
    public string   CityName       { get; set; } = "";
    public long     UniqueBettors  { get; set; }
    public long     TotalBets      { get; set; }
    public decimal  TotalStaked    { get; set; }
    public decimal  TotalLiability { get; set; }
    public decimal  WinStaked      { get; set; }
    public decimal  PlaceStaked    { get; set; }
}