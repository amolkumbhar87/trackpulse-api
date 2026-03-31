using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Jockey.cs
[Table("jockeys")]
public class Jockey
{
    [Key, Column("jockey_id")]
    public int JockeyId { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("license_no")]
    public string? LicenseNo { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}