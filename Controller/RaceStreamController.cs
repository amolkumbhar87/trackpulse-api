using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

[ApiController]
[Route("api/[controller]")]
public class RaceStreamController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public RaceStreamController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpPost("stream-url/{raceId}")]
    public async Task<IActionResult> UpdateStreamUrl(int raceId, [FromBody] UpdateStreamUrlRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.StreamUrl))
        {
            return BadRequest(new { error = "Stream URL cannot be empty" });
        }
        
        var existingStream = await _context.RaceStreams
            .FirstOrDefaultAsync(rs => rs.RaceId == raceId);
        
        if (existingStream != null)
        {
            existingStream.StreamUrl = request.StreamUrl;
            existingStream.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var newStream = new RaceStream
            {
                RaceId = raceId,
                StreamUrl = request.StreamUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await _context.RaceStreams.AddAsync(newStream);
        }
        
        await _context.SaveChangesAsync();
        
        return Ok(new { 
            message = "Stream URL updated successfully", 
            raceId = raceId, 
            streamUrl = request.StreamUrl 
        });
    }
}

public class UpdateStreamUrlRequest
{
    public string StreamUrl { get; set; }
}