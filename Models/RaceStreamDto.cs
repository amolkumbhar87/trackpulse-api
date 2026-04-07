public class RaceStreamDto
{
    public int Id { get; set; }
    public int? RaceId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string StreamUrl { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Venue { get; set; }
    public DateTime? RaceDate { get; set; }
    public string? RaceName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}