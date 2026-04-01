    
// User.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("users")]
public class User
{
    [Key, Column("user_id")]
    public int UserId { get; set; }

    [Column("user_name")]
    public string UserName { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("password_hash")]
    public string PasswordHash { get; set; }

    [Column("role")]
    public string Role { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("session_token")]
    public string? SessionToken { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    [Column("mobile_number")]
    [MaxLength(20)]
    public string MobileNumber { get; set; }

     [Column("status")]
    public string Status { get; set; }
}


















