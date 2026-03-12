using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _db;
    public HealthController(AppDbContext db) => _db = db;

    [HttpGet("db")]
    public async Task<IActionResult> CheckDb()
    {
        try
        {
            // Checks actual connection to Supabase
            var canConnect = await _db.Database.CanConnectAsync();

            // Checks users table exists and is queryable
            var userCount = await _db.Users.CountAsync();

            return Ok(new
            {
                status     = "Connected",
                canConnect = canConnect,
                userCount  = userCount,
                database   = _db.Database.GetConnectionString()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status  = "Failed",
                error   = ex.Message
            });
        }
    }
}