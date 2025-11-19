using MiniCloudNote.Core.Entities; // Phải có
using MiniCloudNote.Core.Interfaces;
using System.Threading.Tasks;       // Phải có

namespace MiniCloudNote.Infrastructure
{
    // Thêm ": INoteRepository" để triển khai interface
    public class NoteRepository : INoteRepository
    {
        public async Task<Note> SaveAsync(Note note)
        {
            Console.WriteLine("Đang kết nối tới PostgreSQL (Async)...");
            Console.WriteLine($"Đã lưu: Title = {note.Title}, ID = {note.Id}");

            await Task.Delay(10); 

            return note;
        }
    }
}