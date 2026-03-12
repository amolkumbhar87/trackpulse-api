
// City.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("city")]
public class City
{
    [Key, Column("city_id")]
    public int CityId { get; set; }

    [Column("city_name")]
    public string CityName { get; set; }

    [Column("state")]
    public string? State { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    public ICollection<Venue> Venues { get; set; }
}