using Microsoft.EntityFrameworkCore;
using MiniCloudNote.Core.Entities; 
using MiniCloudNote.Core.Interfaces;
using MiniCloudNote.Infrastructure.Data;
using System.Threading.Tasks;  

namespace MiniCloudNote.Infrastructure.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _context;

        public NoteRepository(AppDbContext context)
        {
            _context = context;
        }

      
        // Dùng FindAsync để tìm ghi chú theo ID (Nhanh và gọn)
        public async Task<Note?> GetByIdAsync(Guid id)
        {
            return await _context.Notes.FindAsync(id);
        }

        // Lọc ghi chú theo OwnerId
        public async Task<IEnumerable<Note>> GetAllByOwnerIdAsync(Guid ownerId)
        {
            return await _context.Notes
            .Where(n => n.OwnerId == ownerId)
            .OrderByDescending(n => n.CreatedAt) // Mới nhất lần đầu
            .ToListAsync();
        }
        // Thêm ghi chú vào DbContext và lưu thay đổi
        public async Task<Note> AddAsync(Note note)
        {
            await _context.Notes.AddAsync(note);
            await _context.SaveChangesAsync();
            return note;
        }

        public async Task UpdateAsync(Note note)
        {
            _context.Notes.Update(note); // Đánh dấu đối tượng này đã bị sửa
            await _context.SaveChangesAsync(); // Gửi lệnh UPDATE SQL xuống DB
        }

        public async Task DeleteAsync(Note note)
        {
            _context.Notes.Remove(note); // Đánh dấu xóa
            await _context.SaveChangesAsync(); // Gửi lệnh DELETE SQL xuống DB
        }
    }
}