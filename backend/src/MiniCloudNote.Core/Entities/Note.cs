using System.ComponentModel.DataAnnotations; // Cần cho [Key]
using System; // Cần cho Guid và DateTime
using System.ComponentModel.DataAnnotations.Schema; // Cần cho [ForeignKey]
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

        // --- 2 DÒNG NÀY CHO TÍNH NĂNG THÙNG RÁC ---
        public bool IsDeleted { get; set; } = false; // false = Bình thường, true = Đã vào thùng rác
        public DateTime? DeletedAt { get; set; } // Lưu lại thời điểm bị ném vào thùng rác

        public bool IsPinned { get; set; } = false; // Mặc định tạo ra là không ghim
        
        // Khóa ngoại lưu ID của user tạo ghi chú
        public Guid OwnerId { get; set; }

        // Navigation property: Giúp EF Core hiểu mối quan hệ và join bảng dễ dàng
        // "virtual" để hỗ trợ lazy loading nếu sau này cần
        [ForeignKey("OwnerId")]
        public virtual User? Owner { get; set; }
    }
}
