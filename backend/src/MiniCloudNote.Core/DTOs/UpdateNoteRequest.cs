using System.ComponentModel.DataAnnotations;

namespace MiniCloudNote.Core.DTOs
{
    // Giả lập class DTO để nhận dữ liệu cập nhật note từ client
    public class UpdateNoteRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}