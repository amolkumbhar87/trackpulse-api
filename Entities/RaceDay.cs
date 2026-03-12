using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// RaceDay.cs
[Table("race_day")]
public class RaceDay
{
    [Key, Column("race_day_id")]
    public int RaceDayId { get; set; }

    [Column("venue_id")]
    public int VenueId { get; set; }

    [Column("race_date")]
    public DateOnly RaceDate { get; set; }

    [Column("status")]
    public string Status { get; set; } = "Upcoming";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Venue Venue { get; set; }
    public ICollection<Race> Races { get; set; }
}