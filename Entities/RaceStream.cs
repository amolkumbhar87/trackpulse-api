
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("RaceStreams")]
public class RaceStream
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    
    [Column("RaceId")]  
    public int RaceId { get; set; } 
    [Column("StreamUrl")]
    public string StreamUrl { get; set; } = null!;
    
    [Column("CreatedAt")]

    public DateTime CreatedAt { get; set; }

    [Column("UpdatedAt")]

    public DateTime UpdatedAt { get; set; }

    [Column("IsActive")]
    public bool IsActive { get; set; }

}