using System.Collections.Concurrent;

namespace MiniCloudNote.Core.DTOs
{
    public class NoteResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsPinned { get; set; }

        // Không trả về IsDeleted và DeletedAt để client không cần quan tâm đến thùng rác
        // Không trả về OwnerId để bảo mật, user tự biết là của mình
    }
}