using Microsoft.EntityFrameworkCore;
using MiniCloudNote.Core.Entities;
using MiniCloudNote.Core.Interfaces;
using MiniCloudNote.Infrastructure.Data;

namespace MiniCloudNote.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}