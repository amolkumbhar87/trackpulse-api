using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Race.cs
[Table("race")]
public class Race
{
    [Key, Column("race_id")]
    public int RaceId { get; set; }

    [Column("race_day_id")]
    public int RaceDayId { get; set; }

    [Column("race_number")]
    public int RaceNumber { get; set; }

    [Column("race_name")]
    public string RaceName { get; set; }

    [Column("race_type")]
    public string? RaceType { get; set; }

    [Column("distance_meters")]
    public int? DistanceMeters { get; set; }

    [Column("start_time")]
    public DateTime? StartTime { get; set; }

    [Column("status")]
    public string Status { get; set; } = "Upcoming";

    public RaceDay RaceDay { get; set; }
    public ICollection<RaceHorse> RaceHorses { get; set; }
}
