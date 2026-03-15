public interface IUserRepository
{
    Task<User> GetByUsernameAsync(string username);
    Task<User> CreateAsync(User user);
    Task<User> GetByMobileNumberAsync(string mobileNumber);
}