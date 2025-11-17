using MiniCloudNote.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MiniCloudNote.Core.Services
{
    public class NoteService : INoteService
    {
        // === BẮT ĐẦU CODE OCP === //
        // 1.Service không tự tạo, mà yêu cầu "tiêm" vào 1 danh sách chiến lược
        private readonly IEnumerable<IFormattingStrategy> _formattingStrategies;
        
        // 2.Sửa Constructor (Hàm khởi tạo)
        public NoteService(IEnumerable<IFormattingStrategy> formattingStrategies)
        {
            _formattingStrategies = formattingStrategies;
        }

        public bool CreateNote(string title, string content)
        {
            // === TRÁCH NHIỆM 1: Nghiệp vụ (đã chuyển về đây) ===
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentException("Tiêu đề là bắt buộc.");
            }
            if (content?.Length > 1000)
            {
                throw new ArgumentException("Nội dung quá dài.");
            }

            // TODO: Gọi Repository để lưu
            // TODO: Gọi EmailService để gửi

            return true;
        }

        // === SỬA LẠI HOÀN TOÀN HÀM NÀY THEO OCP === //
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
    }
}