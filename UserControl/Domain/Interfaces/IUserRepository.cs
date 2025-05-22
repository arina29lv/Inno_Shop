using UserControl.Domain.Models;

namespace UserControl.Domain.Interfaces;

public interface IUserRepository
{
    Task<int> AddUserAsync(User user);
    
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
}