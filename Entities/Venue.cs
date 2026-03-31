// Venue.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("venues")]
public class Venue
{
    [Key, Column("venue_id")]
    public int VenueId { get; set; }

    [Column("city_id")]
    public int CityId { get; set; }    

    [Column("venue_name")]
    public string VenueName { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public City City { get; set; }
    public ICollection<RaceDay> RaceDays { get; set; }
}