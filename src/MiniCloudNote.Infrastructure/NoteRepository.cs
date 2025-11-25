using Microsoft.EntityFrameworkCore;
using MiniCloudNote.Core.Entities; 
using MiniCloudNote.Core.Interfaces;
using MiniCloudNote.Infrastructure.Data;
using System.Threading.Tasks;  

namespace MiniCloudNote.Infrastructure
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _context;

        public NoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Note> SaveAsync(Note note)
        {
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }

        // 1. Lấy tất cả : Dùng ToListAsync để lấy tất cả ghi chú từ cơ sở dữ liệu
        public async Task<IEnumerable<Note>> GetAllAsync()
        {
            return await _context.Notes.ToListAsync();
        }

        // 2. Lấy theo ID: Dùng FindAsync để tìm ghi chú theo ID (Nhanh và gọn)
        public async Task<Note?> GetByIdAsync(Guid id)
        {
            return await _context.Notes.FindAsync(id);
        }

        // 3.Update: EF Core rất thông minh, chỉ cần đánh dấu là "Modified"
        public async Task UpdateAsync(Note note)
        {
            _context.Notes.Update(note); // Đánh dấu đối tượng này đã bị sửa
            await _context.SaveChangesAsync(); // Gửi lệnh UPDATE SQL xuống DB
        }

        // 4. Delete: Đánh dấu là "Deleted"
        public async Task DeleteAsync(Note note)
        {
            _context.Notes.Remove(note); // Đánh dấu xóa
            await _context.SaveChangesAsync(); // Gửi lệnh DELETE SQL xuống DB
        }
    }
}