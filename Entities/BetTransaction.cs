using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// BetTransaction.cs
[Table("bet_transaction")]
public class BetTransaction
{
    [Key, Column("transaction_id")]
    public int TransactionId { get; set; }

    [Column("bet_id")]
    public int BetId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("transaction_type")]
    public string TransactionType { get; set; }

    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("payment_status")]
    public string PaymentStatus { get; set; } = "Pending";

    [Column("reference_no")]
    public string? ReferenceNo { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Bet Bet { get; set; }
    public User User { get; set; }
}