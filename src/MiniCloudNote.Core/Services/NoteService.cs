using MiniCloudNote.Core.Interfaces;

namespace MiniCloudNote.Core.Services
{
    public class NoteService : INoteService
    {
        // Tạm thời để trống, chúng ta sẽ thêm Repository và Email sau
        public NoteService()
        {
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
    }
}