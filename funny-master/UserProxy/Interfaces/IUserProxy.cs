using UserProxy.Models;
namespace UserProxy.Interfaces
{
    public interface IUserProxy
    {
        Task<bool> ExistsAsync(string id);
        Task<UserDto?> GetUserAsync(string id);
        Task<UserDto?> CreateUserAsync(UserDto user);
        Task<UserDto?> UpdateUserAsync(UserDto user);
    }
}
