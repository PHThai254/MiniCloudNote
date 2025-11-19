namespace MiniCloudNote.Core.Interfaces
{
    // Interface cho người chỉ được xem
    public interface IReadOnlyNote
    {
        string Title { get; }
        string Content { get; }
        DateTime CreatedAt { get; }
    }

    // Interface cho người được phép sửa (Kế thừa từ ReadOnly, thêm khả năng Ghi)
    public interface IEditableNote : IReadOnlyNote
    {
        // Ghi đè (new) để thêm setter
        new string Title { get; set; }
        new string Content { get; set; }
    }
}