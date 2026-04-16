public class RaceSummaryDto
{
    public int RaceHorseId { get; set; }
    public int DrawNumber { get; set; }
    public string HorseName { get; set; } = "";
    public int? Position { get; set; }
    public string BetType { get; set; } = "";
    public long BetCount { get; set; }
    public decimal TotalStaked { get; set; }
    public string Result { get; set; } = "";

    public string RaceName { get; set; }


    // Win bets summary
    public decimal WinStake { get; set; }
    public long WinBetCount { get; set; }
    public decimal WinPayout { get; set; }

    // Place bets summary
    public decimal PlaceStake { get; set; }
    public long PlaceBetCount { get; set; }
    public decimal PlacePayout { get; set; }

    // Combined totals
    public decimal TotalStake { get; set; }
    public long TotalBets { get; set; }
    public decimal TotalPayout { get; set; }

    // Calculated properties
    public decimal AverageWinOdds => WinBetCount > 0 ? WinPayout / WinStake : 0;
    public decimal AveragePlaceOdds => PlaceBetCount > 0 ? PlacePayout / PlaceStake : 0;
    public decimal WinLiability => WinPayout - WinStake;
    public decimal PlaceLiability => PlacePayout - PlaceStake;
    public decimal TotalLiability => TotalPayout - TotalStake;
}