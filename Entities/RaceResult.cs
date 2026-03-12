using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// RaceResult.cs
[Table("race_result")]
public class RaceResult
{
    [Key, Column("result_id")]
    public int ResultId { get; set; }

    [Column("race_id")]
    public int RaceId { get; set; }

    [Column("race_horse_id")]
    public int RaceHorseId { get; set; }

    [Column("finish_position")]
    public int FinishPosition { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Race Race { get; set; }
    public RaceHorse RaceHorse { get; set; }
}