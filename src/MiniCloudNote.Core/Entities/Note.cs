using System.ComponentModel.DataAnnotations; // Cần cho [Key]
using MiniCloudNote.Core.Interfaces; 

namespace MiniCloudNote.Core.Entities
{
    // Note thực thi IEditableNote (có thể đọc và ghi)
    public class Note : IEditableNote
    {
        [Key] // Đánh dấu đây là khóa chính
        public Guid Id { get; set; } // Dùng Guid để ID là duy nhất trên toàn hệ thống

        [Required] // Tiêu đề là bắt buộc
        [MaxLength(255)] // Giới hạn độ dài tiêu đề
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty; // Nội dung ghi chú
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời gian tạo ghi chú
        public DateTime? UpdatedAt { get; set; } // Thời gian cập nhật ghi chú, có thể null

        // TODO: Sẽ thêm UserId ở đây khi làm về Auth
        
    }
}
