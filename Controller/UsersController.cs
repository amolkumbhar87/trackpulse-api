using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequestDto request)
    {
        if (request == null || string.IsNullOrEmpty(request.MobileNumber) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Mobile and password are required");
        }

        var user = await _userRepository.GetByMobileNumberAsync(request.MobileNumber);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid mobile number or password.");
        return Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto userRequest)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(userRequest.UserName);
        if (existingUser != null) return BadRequest("Username already exists.");       

        var createdUser = await _userRepository.CreateAsync(userRequest);
        return Ok(new { createdUser.UserId, createdUser.UserName });
    }

    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetByUsernameAsync(string username)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return NotFound($"User '{username}' not found");
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetUsersByStatusAsync(string status, [FromQuery]string? mobileNumber=null)
    {
        try
        {
            var users = await _userRepository.GetUsersByStatusAsync(status, mobileNumber);
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }

    [HttpPatch("status/{userId}")]
    public async Task<IActionResult> UpdateUserStatusAsync(int userId, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var users = await _userRepository.UpdateUserStatusAsync(request.Status, userId);
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, details = ex.InnerException?.Message });
        }
    }
}
            
