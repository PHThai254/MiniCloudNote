using MiniCloudNote.Core.Entities;
using MiniCloudNote.Core.DTOs;

namespace MiniCloudNote.Core.Interfaces
{
    public interface INoteService
    {
        // Lấy danh sách ghi chú của user (trả về DTO NoteResponse)
        Task<PagedResult<NoteResponse>> GetUserNotesAsync(Guid userId, NoteQueryParameters query);

        // Lấy chi tiết 1 ghi chú (Cần userId để đảm bảo chính chủ)
        Task<NoteResponse?> GetNoteByIdAsync(Guid userId, Guid noteId);

        // Tạo mới
        Task<NoteResponse> CreateNoteAsync(Guid userId, CreateNoteRequest request);

        // Cập nhật
        Task<bool> UpdateNoteAsync(Guid noteId, Guid userId, UpdateNoteRequest request);

        // Xóa
        Task<bool> DeleteNoteAsync(Guid noteId, Guid userId);

        Task<PagedResult<NoteResponse>> GetTrashNotesAsync(Guid userId, NoteQueryParameters query);

        // Xoá mềm (Soft Delete - Chuyển vào thùng rác)
        Task<bool> RestoreNoteAsync(Guid noteId, Guid userId);

        // Xóa vĩnh viễn (Hard Delete)
        Task<bool> HardDeleteNoteAsync(Guid noteId, Guid userId);

        // Đảo trạng thái ghim
        Task<bool> TogglePinNoteAsync(Guid noteId, Guid userId);

    }
}
