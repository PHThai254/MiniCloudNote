using MiniCloudNote.Core.Entities;

namespace MiniCloudNote.Core.Interfaces
{
    public interface INoteService
    {
        // Sửa lại chữ ký: Nhận vào dữ liệu thô, trả về Note (Entity)
        // Dùng Task<> để làm Bất đồng bộ (Async) - Best practice
        Task<Note> CreateNoteAsync(string title, string content);
        Task<IEnumerable<Note>> GetAllNotesAsync();
        Task<Note?> GetNoteByIdAsync(Guid id);
        Task UpdateNoteAsync(Guid id, string title, string content);
        Task DeleteNoteAsync(Guid id);
        string FormatNoteContent(string content, string formatType);
    }
}
