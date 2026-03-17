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
        //|| !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)
        if (user == null )
            return Unauthorized("Invalid mobile number or password.");
        return Ok(user);
    }

    // [HttpPost("users/register")]
    // public async Task<IActionResult> Register(UserRegisterDto dto)
    // {
    //     var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
    //     if (existingUser != null) return BadRequest("Username already exists.");

    //     var user = new User
    //     {
    //         Username = dto.Username,
    //         PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
    //         Role = "User"
    //     };

    //     var createdUser = await _userRepository.CreateAsync(user);
    //     return Ok(new { createdUser.Id, createdUser.Username });
    // }

    [HttpGet("{username}")]
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
}