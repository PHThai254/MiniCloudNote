using Microsoft.EntityFrameworkCore;
using MiniCloudNote.Core.Entities;

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
    }
}