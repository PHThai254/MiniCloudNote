using MiniCloudNote.Core.Entities; 
using MiniCloudNote.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace MiniCloudNote.Core.Services
{
    public class NoteService : INoteService
    {
        // === BẮT ĐẦU CODE OCP === //
        // Service không tự tạo, mà yêu cầu "tiêm" vào 1 danh sách chiến lược
        private readonly IEnumerable<IFormattingStrategy> _formattingStrategies;

        // === TIÊM REPOSITORY (SRP/DIP) === //
        private readonly INoteRepository _noteRepository; 
        
        // Sửa Constructor (Hàm khởi tạo) để nhận 2 thứ
        public NoteService(IEnumerable<IFormattingStrategy> formattingStrategies, INoteRepository noteRepository)
        {
            _formattingStrategies = formattingStrategies;
            _noteRepository = noteRepository;
        }

        // === TRIỂN KHAI HÀM MỚI ===
        public async Task<Note> CreateNoteAsync(string title, string content)
        {
            // 1. Trách nhiệm Nghiệp vụ (Validate)
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("Tiêu đề là bắt buộc.");
            }
            if (content?.Length > 1000)
            {
                throw new ArgumentException("Nội dung quá dài.");
            }

            // 2. Tạo Entity
#pragma warning disable CS8601 // Possible null reference assignment.
            var newNote = new Note
            {
                Id = Guid.NewGuid(),
                Title = title,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
#pragma warning restore CS8601 // Possible null reference assignment.

            // TODO: Gọi Repository để lưu
            //_noteRepository.Save(newNote);
            // 3. Gọi Repository để lưu (Vẫn là giả lập)
            // Dùng await vì hàm là Async
            var createdNote = await _noteRepository.SaveAsync(newNote); 

            // TODO: Gọi EmailService để gửi

            // 4. Gọi Email (Tạm thời bỏ qua)
            // _emailService.SendEmail(createdNote.Title);
            return createdNote;
        }

        // 1. Lấy tất cả ghi chú
        public async Task<IEnumerable<Note>> GetAllNotesAsync()
        {
            return await _noteRepository.GetAllAsync();
        }
        // 2. Lấy theo ID
        public async Task<Note?> GetNoteByIdAsync(Guid id)
        {
            return await _noteRepository.GetByIdAsync(id);
        }

        // 3. Cập nhật (QUAN TRỌNG: Tư duy Tìm --> Sửa --> Lưu)
        public async Task UpdateNoteAsync(Guid id, string title, string content)
        {
            // Bước 1: Tìm note cũ theo ID (cũng như tìm món đồ cũ trong kho)
            var existingNote = await _noteRepository.GetByIdAsync(id);
            if (existingNote == null)
            {
                throw new KeyNotFoundException("Không tìm thấy ghi chú để sửa.");
            }

            // Bước 2: Sửa content trên note đó (Modify) (Cũng như sửa thông tin trên món đồ đó)
            existingNote.Title = title;
            existingNote.Content = content;
            existingNote.UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian sửa

            // Bước 3: Lưu lại note đó (Cũng như bảo thủ kho cất lại)
            await _noteRepository.UpdateAsync(existingNote);   
        }

        // 4. Xóa (Tư duy tìm -> Xóa)
        public async Task DeleteNoteAsync(Guid id)
        {
            // Bước 1: Phải tìm thấy mới xóa được
            var existingNote = await _noteRepository.GetByIdAsync(id);
            if (existingNote == null)
            {
                throw new KeyNotFoundException("Không tìm thấy ghi chú để xóa.");
            }

            // Bước 2: Xóa
            await _noteRepository.DeleteAsync(existingNote);    
        }
        public string FormatNoteContent(string content, string formatType)
        {
            // 3. Tìm chiến lược phù hợp trong danh sách
            var strategy = _formattingStrategies.FirstOrDefault(s => s.FormatType == formatType);

            if (strategy != null)
            {
                // 4. Nếu tìm thấy --> Dùng nó (Đây là Đa hình)
                return strategy.Format(content);
            }
            
            // 5. Nếu không tìm thấy chiến lược nào
            throw new NotSupportedException($"Định dạng '{formatType}' không được hỗ trợ.");    

        } 

        // Hàm này CHỈ chấp nhận IReadOnlyNote
        // Dù bạn truyền Note (Entity) vào, hàm này cũng chỉ nhìn thấy phần "Read"
        public string GeneratePreview(IReadOnlyNote note)
        {
            // note.Title = "Sửa bậy"; // --> LỖI BIÊN DỊCH NGAY LẬP TỨC!
            // C# sẽ gạch đỏ dòng trên vì IReadOnlyNote không có setter.    
            return $"Preview: {note.Title} - {note.CreatedAt}";
        }
    }
}