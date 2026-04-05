public interface IUserRepository
{
    Task<User> GetByUsernameAsync(string username);
    Task<User> CreateAsync(UserDto user);
    Task<User> GetByMobileNumberAsync(string mobileNumber);

    Task<IEnumerable<User>> GetUsersByStatusAsync(string status, string mobileNumber=null);

    Task<bool> UpdateUserStatusAsync(string status, int userId);
    Task<decimal> GetWalletBalanceAsync(int userId);

    Task<bool> DeductAsync(int userId, decimal amount);
}