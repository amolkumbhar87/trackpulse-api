
using Dapper;
using Microsoft.EntityFrameworkCore;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    private readonly DapperContext _dapper;
    public UserRepository(AppDbContext db, DapperContext dapper) 
    { _db = db; _dapper = dapper; }


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
            Email = user.Email,
            Status = "new"


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

    public Task<decimal> GetWalletBalanceAsync(int userId)
    {
        var connection = _dapper.CreateConnection();
        const string sql = @"
            SELECT wallet_balance
            FROM trackpulse.users 
            WHERE user_id = @UserId";
        return connection.QuerySingleAsync<decimal>(sql, new { UserId = userId });
    }

    public Task<bool> DeductAsync(int userId, decimal amount)
    {
        var connection = _dapper.CreateConnection();
        const string sql = @"
                UPDATE trackpulse.users 
                SET wallet_balance = wallet_balance - @Amount 
                WHERE user_id = @UserId AND wallet_balance >= @Amount";
        return connection.ExecuteAsync(sql, new { UserId = userId, Amount = amount })
        .ContinueWith(task => task.Result > 0);
    }
}