using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Bet.cs
[Table("bet")]
public class Bet
{
    [Key, Column("bet_id")]
    public int BetId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("race_horse_id")]
    public int RaceHorseId { get; set; }

    [Column("bet_type")]
    public string BetType { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("odds_at_bet")]
    public decimal OddsAtBet { get; set; }

    [Column("status")]
    public string Status { get; set; } = "Pending";

    [Column("placed_at")]
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; }
    public RaceHorse RaceHorse { get; set; }
    public ICollection<BetTransaction> Transactions { get; set; }
}

