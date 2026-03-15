    
using Microsoft.EntityFrameworkCore;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> GetByMobileNumberAsync(string mobileNumber)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber);
    }

    public async Task<User> CreateAsync(User user)
    {
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        if (existingUser != null) return existingUser;

        user = new User
        {
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            Role = user.Role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }
}