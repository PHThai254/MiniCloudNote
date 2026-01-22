using MiniCloudNote.Core.DTOs;      // Dùng DTO từ Core
using MiniCloudNote.Core.Entities;  // Dùng Entity từ Core
using MiniCloudNote.Core.Interfaces; // Dùng Interface từ Core

namespace MiniCloudNote.Infrastructure.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        // Constructor Injection
        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        // 1. Lấy danh sách ghi chú của User
        public async Task<IEnumerable<NoteResponse>> GetUserNotesAsync(Guid userId)
        {
            // Gọi Repo lấy Entity
            var notes = await _noteRepository.GetAllByOwnerIdAsync(userId);
            
            // Map từ Entity -> Response DTO (Ẩn thông tin nhạy cảm)
            return notes.Select(n => new NoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt
            });
        }

        // 2. Lấy chi tiết 1 ghi chú (Có kiểm tra quyền sở hữu)
        public async Task<NoteResponse?> GetNoteByIdAsync(Guid noteId, Guid userId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);
            
            // Logic quan trọng: Nếu note không tồn tại HOẶC không phải của user này -> Trả về null
            if (note == null || note.OwnerId != userId) return null;

            return new NoteResponse
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt
            };
        }

        // 3. Tạo ghi chú mới
        public async Task<NoteResponse> CreateNoteAsync(Guid userId, CreateNoteRequest request)
        {
            var newNote = new Note
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                OwnerId = userId, // <--- QUAN TRỌNG: Gán chủ sở hữu ở đây
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _noteRepository.AddAsync(newNote);

            return new NoteResponse
            {
                Id = newNote.Id,
                Title = newNote.Title,
                Content = newNote.Content,
                CreatedAt = newNote.CreatedAt,
                UpdatedAt = newNote.UpdatedAt
            };
        }

        // 4. Cập nhật ghi chú
        public async Task<bool> UpdateNoteAsync(Guid noteId, Guid userId, UpdateNoteRequest request)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);
            
            // Kiểm tra quyền sở hữu trước khi sửa
            if (note == null || note.OwnerId != userId) return false;

            // Cập nhật thông tin
            note.Title = request.Title;
            note.Content = request.Content;
            note.UpdatedAt = DateTime.UtcNow;

            await _noteRepository.UpdateAsync(note);
            return true;
        }

        // 5. Xóa ghi chú
        public async Task<bool> DeleteNoteAsync(Guid noteId, Guid userId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);

            // Kiểm tra quyền sở hữu trước khi xóa
            if (note == null || note.OwnerId != userId) return false;

            await _noteRepository.DeleteAsync(note);
            return true;
        }
    }
}