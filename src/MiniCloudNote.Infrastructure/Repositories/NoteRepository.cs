using Microsoft.EntityFrameworkCore;
using MiniCloudNote.Core.DTOs;
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

        // Lọc ghi chú theo phân trang
        public async Task<PagedResult<Note>> GetPagedAsync(Guid ownerId, NoteQueryParameters query)
        {
            // 1. Khởi tạo câu truy vấn (Queryable)
            // Lúc này chưa bắn lệnh xuống Database, chỉ mới "lên kế hoạch"
            var queryable = _context.Notes
                                    .AsNoTracking() // Tối ưu hiệu năng cho thao tác chỉ đọc
                                    .Where(n => n.OwnerId == ownerId); // Luôn luôn đọc theo User trước tiên
            // 2. Tìm kiếm (Search)
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.ToLower().Trim();
                // Tìm trong Title Hoặc Content (chứa từ khóa)
                queryable = queryable.Where(n => n.Title.ToLower().Contains(term));
            }
            // BƯỚC 3: Sắp xếp (Sorting)
            // switch expression (C# 8.0+)
            queryable = query.SortBy switch
            {
                "title_asc" => queryable.OrderBy(n => n.Title),
                "title_desc" => queryable.OrderByDescending(n => n.Title),
                "created_asc" => queryable.OrderBy(n => n.CreatedAt),
                _ => queryable.OrderByDescending(n => n.CreatedAt) // Mặc định: Mới nhất lên đầu
            };

            // BƯỚC 4: Đếm tổng số bản ghi (Total Count)
            // Phải đếm sau khi lọc Search, nhưng TRƯỚC khi cắt trang
            var totalCount = await queryable.CountAsync();

            // BƯỚC 5: Phân trang (Paging)
            // Công thức kinh điển: Bỏ qua (Trang hiện tại - 1) * Kích thước trang
            var items = await queryable
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(); // Lúc này mới thực sự bắn lệnh SQL xuống DB lấy dữ liệu về

            // BƯỚC 6: Đóng gói kết quả
            return new PagedResult<Note>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

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