using MiniCloudNote.Core.Entities;

namespace MiniCloudNote.Core.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(User user, string password);
        Task<string?> LoginAsync(string username, string password); // Trả về JWT Token string
    }
}