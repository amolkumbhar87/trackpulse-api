public class UserDto
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }

    public string Role { get; set; }
    public bool IsActive { get; set; } = true;



    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    public string MobileNumber { get; set; }
}