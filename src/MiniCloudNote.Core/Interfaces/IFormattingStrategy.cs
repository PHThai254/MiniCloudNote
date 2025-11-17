namespace MiniCloudNote.Core.Interfaces
{
    public interface IFormattingStrategy
    {
        // Mỗi chiến lược phải cho biết nó xử lý loại nào
        string FormatType { get; }
        // Mỗi chiến lược phải có logic xử lý riêng
        string Format(string content);
    }
}

