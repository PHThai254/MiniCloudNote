using Microsoft.EntityFrameworkCore;
using MiniCloudNote.Core.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace MiniCloudNote.Infrastructure.Data
{
    // Kế thừa từ DbContext của EF Core
    public class AppDbContext : DbContext
    {
        // Constructor nhận options (chuỗi kết nối, v.v.) và truyền cho lớp cha
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // Khai báo bảng Notes. Tên biến 'Notes' sẽ là tên bảng trong PostgreSQL
        public DbSet<Note> Notes { get; set; }
        // Thêm dòng này vào trong class
        public DbSet<User> Users { get; set; }
    }
}