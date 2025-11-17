using MiniCloudNote.Core.Interfaces;

namespace MiniCloudNote.Core.Services.FormattingStrategies
{
    public class HtmlFormattingStrategy : IFormattingStrategy
    {
        public string FormatType => "Html";

        public string Format(string content)
        {
            // Logic format Html
            // Bao bọc nội dung trong thẻ <p>
            return $"<p>{content}</p>";
        }
    }
}