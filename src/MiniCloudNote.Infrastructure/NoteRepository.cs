namespace MiniCloudNote.Infrastructure
{
    public class NoteRepository // Sẽ implement INoteRepository ở bài DIP
    {
        public void Save(string title, string content)
        {
            // === TRÁCH NHIỆM 2: Database (đã chuyển về đây) ===
            Console.WriteLine("Đang kết nối tới PostgreSQL...");
            Console.WriteLine($"Đã lưu: Title = {title}, Content = {content}");
        }
    }
}