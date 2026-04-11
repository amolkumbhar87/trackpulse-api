using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// RaceHorse.cs
[Table("race_horses")]
public class RaceHorse
{
    [Key, Column("race_horse_id")]
    public int RaceHorseId { get; set; }

    [Column("race_id")]
    public int RaceId { get; set; }

    [Column("horse_id")]
    public int HorseId { get; set; }

    [Column("jockey_id")]
    public int? JockeyId { get; set; }

    [Column("trainer_id")]
    public int TrainerId { get; set; }

    [Column("draw_number")]
    public int DrawNumber { get; set; }

    [Column("weight")]
    public decimal? Weight { get; set; }

    [Column("rating")]
    public int? Rating { get; set; }


    [Column("position")]
    public int? Position { get; set; }

    public Race Race { get; set; }
    public Horse Horse { get; set; }
    public Jockey? Jockey { get; set; }

    public Trainer? Trainer { get; set; }
    public ICollection<Odds> Odds { get; set; }
}