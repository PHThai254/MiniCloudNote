using MiniCloudNote.Core.Interfaces;

namespace MiniCloudNote.Core.Services.FormattingStrategies
{
    public class MarkdownFormattingStrategy : IFormattingStrategy
    {
        public string FormatType => "Markdown";

        public string Format(string content)
        {
            // Logic format Markdown
            return $"**{content}**";
        }
    }
}