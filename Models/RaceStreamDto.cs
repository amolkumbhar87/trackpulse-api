public class RaceStreamDto
{
    
    public int Id { get; set; }
    public int RaceId { get; set; }
    public string StreamUrl { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}