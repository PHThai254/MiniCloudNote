namespace MiniCloudNote.Core.Interfaces
{
    public interface INoteService
    {
        bool CreateNote(string title, string content);
    }
}