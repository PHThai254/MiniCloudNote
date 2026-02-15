using MiniCloudNote.Core.Entities;

namespace MiniCloudNote.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
    }
}