namespace MiniCloudNote.Core.DTOs
{
    public class NoteResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        // Không trả về OwnerId để bảo mật, user tự biết là của mình
    }
}