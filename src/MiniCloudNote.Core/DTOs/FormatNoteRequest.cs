namespace MiniCloudNote.Core.DTOs
{
    // Hàm định dạng dữ liệu để nhận request
    public class FormatNoteRequest
    {
        public string Content { get; set; } = string.Empty;
        public string FormatType { get; set; } = string.Empty; // e.g., "Markdown", "PlainText"
    }
}