using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Horse.cs
[Table("horses")]
public class Horse
{
    [Key, Column("horse_id")]
    public int HorseId { get; set; }

    [Column("horse_name")]
    public string HorseName { get; set; }

    [Column("age")]
    public int? Age { get; set; }

    [Column("gender")]
    public string? Gender { get; set; }

    [Column("color")]
    public string? Color { get; set; }

    [Column("sire")]
    public string? Sire { get; set; }

    [Column("dam")]
    public string? Dam { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}