using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("race_streams")]
public class RaceStream
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("race_id")]
    public int? RaceId { get; set; }  // Nullable as per schema (INT NULL)

    [Column("title")]
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("stream_url")]
    [Required]
    [MaxLength(500)]
    public string StreamUrl { get; set; } = null!;

    [Column("status")]
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = null!; // 'live' | 'upcoming' | 'recorded'

    [Column("venue")]
    [MaxLength(255)]
    public string? Venue { get; set; }

    [Column("race_date")]
    public DateTime? RaceDate { get; set; }

    [Column("race_name")]
    [MaxLength(255)]
    public string? RaceName { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Note: Your schema has 'Update_dAt' but likely meant 'updated_at'
    // If this is a typo in schema, use 'updated_at' instead
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}