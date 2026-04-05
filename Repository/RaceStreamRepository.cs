
using Microsoft.EntityFrameworkCore;
using System;
public class RaceStreamRepository : IRaceStreamRepository
{
    private readonly AppDbContext _context;

    public RaceStreamRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetStreamUrlAsync(int raceId)
    {
        var raceStream = await _context.RaceStreams
            .Where(rs => rs.RaceId == raceId && rs.IsActive)
            .FirstOrDefaultAsync();

        return raceStream?.StreamUrl;
    }
    public async Task<List<RaceStream>> GetAllActiveStreamsAsync()
    {
        return await _context.RaceStreams
            .Where(rs => rs.IsActive)
            .OrderByDescending(rs => rs.UpdatedAt)
            .ToListAsync();
    }

    public async Task<bool> DeactivateStreamAsync(int raceId)
    {
        var existingStream = await _context.RaceStreams
            .FirstOrDefaultAsync(rs => rs.RaceId == raceId);

        if (existingStream != null)
        {
            existingStream.IsActive = false;
            existingStream.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<RaceStream> GetStreamByIdAsync(int id)
    {
        return await _context.RaceStreams.FindAsync(id);
    }

    public async Task<bool> DeleteStreamAsync(int raceId)
    {
        var existingStream = await _context.RaceStreams
            .FirstOrDefaultAsync(rs => rs.RaceId == raceId);

        if (existingStream != null)
        {
            _context.RaceStreams.Remove(existingStream);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateStreamUrlAsync(int raceId, string streamUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(streamUrl))
                throw new ArgumentException("Stream URL cannot be empty", nameof(streamUrl));

            var existingStream = await _context.RaceStreams
                .FirstOrDefaultAsync(rs => rs.RaceId == raceId);

            if (existingStream != null)
            {
                existingStream.StreamUrl = streamUrl;
                existingStream.UpdatedAt = DateTime.UtcNow;
                // IsActive remains unchanged
            }
            else
            {
                var newStream = new RaceStream
                {
                    RaceId = raceId,
                    StreamUrl = streamUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true  // New streams are active by default
                };
                await _context.RaceStreams.AddAsync(newStream);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error updating stream URL: {ex.Message}");
            return false;
        }
    }
}
