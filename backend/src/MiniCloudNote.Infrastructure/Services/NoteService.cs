using MiniCloudNote.Core.DTOs;      
using MiniCloudNote.Core.Entities;  
using MiniCloudNote.Core.Interfaces; 
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCloudNote.Infrastructure.Services
{
    public class NoteService : INoteService
    {
        private readonly INoteRepository _noteRepository;

        public NoteService(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        // 1. Lấy danh sách ghi chú (Bình thường)
        public async Task<PagedResult<NoteResponse>> GetUserNotesAsync(Guid userId, NoteQueryParameters query)
        {
            var pagedData = await _noteRepository.GetPagedAsync(userId, query);
            
            var noteResponses = pagedData.Items.Select(n => new NoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                IsPinned = n.IsPinned
            });

            return new PagedResult<NoteResponse>
            {
                Items = noteResponses,
                TotalCount = pagedData.TotalCount,
                PageIndex = pagedData.PageIndex,
                PageSize = pagedData.PageSize
            };
        }
        
        // 2. Lấy chi tiết 1 ghi chú
        public async Task<NoteResponse?> GetNoteByIdAsync(Guid noteId, Guid userId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);
            
            // Không cho phép xem nếu không phải chủ sở hữu, hoặc ghi chú ĐÃ BỊ XÓA (nằm trong thùng rác)
            if (note == null || note.OwnerId != userId || note.IsDeleted) return null;

            return new NoteResponse
            {
                Id = note.Id,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                IsPinned = note.IsPinned
            };
        }

        // 3. Tạo ghi chú mới (Giữ nguyên)
        public async Task<NoteResponse> CreateNoteAsync(Guid userId, CreateNoteRequest request)
        {
            var newNote = new Note
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                OwnerId = userId, 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false // Mặc định tạo ra là chưa bị xóa
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
            
            // Không cho phép sửa ghi chú đang nằm trong thùng rác
            if (note == null || note.OwnerId != userId || note.IsDeleted) return false;

            note.Title = request.Title;
            note.Content = request.Content;
            note.UpdatedAt = DateTime.UtcNow;

            await _noteRepository.UpdateAsync(note);
            return true;
        }

        // 5. Chuyển vào Thùng rác (Soft Delete)
        public async Task<bool> DeleteNoteAsync(Guid noteId, Guid userId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);

            if (note == null || note.OwnerId != userId || note.IsDeleted) return false;

            // ĐÁNH DẤU LÀ ĐÃ XÓA THAY VÌ XÓA THẬT
            note.IsDeleted = true;
            note.DeletedAt = DateTime.UtcNow;

            await _noteRepository.UpdateAsync(note); // Update thay vì Delete
            return true;
        }
        // 6. Phục hồi ghi chú từ Thùng rác
        public async Task<bool> RestoreNoteAsync(Guid noteId, Guid userId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);

            // Phải là ghi chú của mình và đang bị xóa thì mới được phục hồi
            if (note == null || note.OwnerId != userId || !note.IsDeleted) return false;

            note.IsDeleted = false;
            note.DeletedAt = null;
            note.UpdatedAt = DateTime.UtcNow; // Cập nhật lại thời gian sửa

            await _noteRepository.UpdateAsync(note);
            return true;
        }

        // 7. Xóa Vĩnh Viễn (Hard Delete) - Dùng khi dọn dẹp thùng rác
        public async Task<bool> HardDeleteNoteAsync(Guid noteId, Guid userId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);

            if (note == null || note.OwnerId != userId) return false;

            // XÓA THẬT KHỎI DATABASE
            await _noteRepository.DeleteAsync(note);
            return true;
        }

        // 8. Lấy danh sách ghi chú trong Thùng rác
        public async Task<PagedResult<NoteResponse>> GetTrashNotesAsync(Guid userId, NoteQueryParameters query)
        {
            var pagedData = await _noteRepository.GetPagedTrashAsync(userId, query);
            
            var noteResponses = pagedData.Items.Select(n => new NoteResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt
            });

            return new PagedResult<NoteResponse>
            {
                Items = noteResponses,
                TotalCount = pagedData.TotalCount,
                PageIndex = pagedData.PageIndex,
                PageSize = pagedData.PageSize
            };
        }

        // 9. Đảo trạng thái Ghim / Bỏ ghim
        public async Task<bool> TogglePinNoteAsync(Guid noteId, Guid userId)
        {
            var note = await _noteRepository.GetByIdAsync(noteId);
            
            // Không cho ghim nếu không tìm thấy, không phải chủ, hoặc đang trong thùng rác
            if (note == null || note.OwnerId != userId || note.IsDeleted) return false;

            note.IsPinned = !note.IsPinned; // Đang true thì thành false, đang false thì thành true
            note.UpdatedAt = DateTime.UtcNow;

            await _noteRepository.UpdateAsync(note);
            return true;
        }
    }
}