using MiniCloudNote.Core.Entities;
using System.Threading.Tasks;

namespace MiniCloudNote.Core.Interfaces
{
    public interface INoteRepository
    {
       // Lấy 1 ghi chú theo ID
        Task<Note?> GetByIdAsync(Guid id);

        // Lấy danh sách ghi chú của MỘT user cụ thể (Quan trọng!)
        Task<IEnumerable<Note>> GetAllByOwnerIdAsync(Guid ownerId);

        // Thêm ghi chú mới (Create): Tạo ghi chú mới trong hệ thống - Như mua hàng mới
        Task<Note> AddAsync(Note note);

        // Cập nhật ghi chú (Update): Ghi đè nội dung cũ  - Như đổi hàng đã mua
        Task UpdateAsync(Note note);

        // Xóa (Delete): Vứt bỏ ghi chú không cần thiết - Như bỏ đồ thừa
        Task DeleteAsync(Note note);
    }
}