namespace MiniCloudNote.API.DTOs
{
    // Giả lập class DTO để nhận dữ liệu tạo note từ client
    public class CreateNoteRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}