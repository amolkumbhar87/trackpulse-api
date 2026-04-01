    
using Microsoft.EntityFrameworkCore;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<User> GetByMobileNumberAsync(string mobileNumber)
    {
        var users = await _db.Users.ToListAsync();
        return users.FirstOrDefault(u => u.MobileNumber == mobileNumber);
    }

    public async Task<IEnumerable<User>> GetUsersByStatusAsync(string status, string mobileNumber=null)
    {
        var users = await _db.Users.ToListAsync();
        return users?.Where(u => u.Status.ToLower() == status);
    }

    
    public async Task<User> CreateAsync(UserDto user)
    {
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
        if (existingUser != null) return existingUser;

       var userEntity = new User
        {
            UserName = user.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
            Role = "User",
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            MobileNumber = user.MobileNumber,
            Email = user.Email


        };

        _db.Users.Add(userEntity);
        await _db.SaveChangesAsync();
        return userEntity;
    }

    public async Task<bool> UpdateUserStatusAsync(string status, int userId)
    {
        var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null) return false;

        user.Status = status;
        user.IsActive = status.ToLower() == "active";
        if (user.CreatedAt.Kind != DateTimeKind.Utc)
    {
        user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);
    }
        _db.Users.Update(user);
        return await Task.FromResult(_db.SaveChanges() > 0);
    }
}