namespace MiniCloudNote.Core.Interfaces
{
    public interface INoteService
    {
        bool CreateNote(string title, string content);
        string FormatNoteContent(string content, string formatType);
    }
}