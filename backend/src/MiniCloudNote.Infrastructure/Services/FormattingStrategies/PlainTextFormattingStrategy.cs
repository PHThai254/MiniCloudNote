using MiniCloudNote.Core.Interfaces;

namespace MiniCloudNote.Infrastructure.Services.FormattingStrategies
{
    public class PlainTextFormattingStrategy : IFormattingStrategy
    {
        public string FormatType => "PlainText";

        public string Format(string content)
        {
            // Logic format PlainText
            return content;
        }
    }
}