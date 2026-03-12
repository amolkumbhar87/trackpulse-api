using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Odds.cs
[Table("odds")]
public class Odds
{
    [Key, Column("odds_id")]
    public int OddsId { get; set; }

    [Column("race_horse_id")]
    public int RaceHorseId { get; set; }

    [Column("win_odds")]
    public decimal? WinOdds { get; set; }

    [Column("place_odds")]
    public decimal? PlaceOdds { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_by")]
    public int UpdatedBy { get; set; }

    public RaceHorse RaceHorse { get; set; }
}