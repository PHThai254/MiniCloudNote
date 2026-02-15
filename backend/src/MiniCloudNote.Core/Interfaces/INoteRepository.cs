using MiniCloudNote.Core.Entities;
using System.Threading.Tasks;
using MiniCloudNote.Core.DTOs;

namespace MiniCloudNote.Core.Interfaces
{
    public interface INoteRepository
    {
       // Lấy 1 ghi chú theo ID
        Task<Note?> GetByIdAsync(Guid id);

        // Lấy danh sách ghi chú của MỘT user cụ thể
        Task<IEnumerable<Note>> GetAllByOwnerIdAsync(Guid ownerId);

        // Lấy danh sách phân trang
        Task<PagedResult<Note>> GetPagedAsync(Guid ownerId, NoteQueryParameters query);

        // Thêm ghi chú mới (Trả về Note để lấy được ID vừa sinh ra)
        Task<Note> AddAsync(Note note);

        // Cập nhật ghi chú
        Task UpdateAsync(Note note);

        // Xóa ghi chú
        Task DeleteAsync(Note note);
    }
}