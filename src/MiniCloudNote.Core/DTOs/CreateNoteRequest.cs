using System.ComponentModel.DataAnnotations;

namespace MiniCloudNote.Core.DTOs
{
    // Giả lập class DTO để nhận dữ liệu tạo note từ client
    public class CreateNoteRequest
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [MaxLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự")]
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}