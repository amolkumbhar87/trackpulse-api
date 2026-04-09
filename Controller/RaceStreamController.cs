using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 

[Route("api/streams")]
// The controller already has [ApiController] and [Route] attributes, but for the new endpoints, we use /api/streams

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RaceStreamController : ControllerBase
{
    private readonly IRaceStreamRepository _raceStreamRepository;

    public RaceStreamController(IRaceStreamRepository raceStreamRepository)
    {
        _raceStreamRepository = raceStreamRepository;
    }

    // GET /api/streams
    [HttpGet]
    public async Task<IActionResult> GetAllStreams()
    {
        var streams = await _raceStreamRepository.GetAllActiveStreamsAsync();
        return Ok(streams);
    }

    // POST /api/streams
    [HttpPost]
    public async Task<IActionResult> AddStream([FromBody] RaceStream request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.StreamUrl))
        {
            return BadRequest(new { error = "Stream URL cannot be empty" });
        }
        request.CreatedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;
        request.IsActive = true;
        var result = await _raceStreamRepository.UpdateStreamUrlAsync(request.RaceId, request.StreamUrl);
        if (result)
        {
            return Ok(new { message = "Stream added successfully", stream = request });
        }
        return StatusCode(500, new { error = "Failed to add stream" });
    }

    // PUT /api/streams/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStream(int id, [FromBody] RaceStream request)
    {
        var stream = await _raceStreamRepository.GetStreamByIdAsync(id);
        if (stream == null)
        {
            return NotFound(new { error = "Stream not found" });
        }
        if (!string.IsNullOrWhiteSpace(request.StreamUrl))
            stream.StreamUrl = request.StreamUrl;
        if (request.IsActive != stream.IsActive)
            stream.IsActive = request.IsActive;
        stream.UpdatedAt = DateTime.UtcNow;
        // Save changes
        var result = await _raceStreamRepository.UpdateStreamUrlAsync(stream.RaceId, stream.StreamUrl);
        if (result)
        {
            return Ok(new { message = "Stream updated successfully", stream });
        }
        return StatusCode(500, new { error = "Failed to update stream" });
    }

    // DELETE /api/streams/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStream(int id)
    {
        var stream = await _raceStreamRepository.GetStreamByIdAsync(id);
        if (stream == null)
        {
            return NotFound(new { error = "Stream not found" });
        }
        var result = await _raceStreamRepository.DeleteStreamAsync(stream.RaceId);
        if (result)
        {
            return Ok(new { message = "Stream deleted successfully" });
        }
        return StatusCode(500, new { error = "Failed to delete stream" });
    }
    
    [HttpPost("stream-url/{raceId}")]
    public async Task<IActionResult> UpdateStreamUrl(int raceId, [FromBody] RaceStream request)
    {
        if (string.IsNullOrWhiteSpace(request.StreamUrl))
        {
            return BadRequest(new { error = "Stream URL cannot be empty" });
        }
        var result = await _raceStreamRepository.UpdateStreamUrlAsync(raceId, request.StreamUrl);
        if (result)
        {
            return Ok(new { message = "Stream URL updated successfully", raceId = raceId, streamUrl = request.StreamUrl });
        }
        return StatusCode(500, new { error = "Failed to update stream URL" });
    }
}

