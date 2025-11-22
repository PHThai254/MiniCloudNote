using MiniCloudNote.Core.Entities;
using System.Threading.Tasks;

namespace MiniCloudNote.Core.Interfaces
{
    public interface INoteRepository
    {
        Task<Note> SaveAsync(Note note);

        // 1. Lấy tất cả ghi chú (Read All) - Như đi siêu thị nhìn lên kệ
        Task<IEnumerable<Note>> GetAllAsync();
        // 2. Lấy chi tiết theo ID (Read by Id) - Như tìm sách theo mã số
        // Trả về null nếu không tìm thấy
        Task<Note?> GetByIdAsync(Guid id);

        // 3. Cập nhật ghi chú (Update): Ghi đè nội dung cũ  - Như đổi hàng đã mua
        Task UpdateAsync(Note note);

        // 4. Xóa (Delete): Vứt bỏ ghi chú không cần thiết - Như bỏ đồ thừa
        Task DeleteAsync(Note note);
    }
}