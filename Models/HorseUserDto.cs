public class HorseUserDto
{
    public int      UserId          { get; set; }
    public string   UserName        { get; set; } = "";
    public string   MobileNumber    { get; set; } = "";
    public string   BetType         { get; set; } = "";
    public decimal  BetAmount       { get; set; }
    public decimal  OddsAtBet       { get; set; }
    public decimal  PotentialPayout { get; set; }
    public string   BetStatus       { get; set; } = "";
    public DateTime PlacedAt        { get; set; }
}