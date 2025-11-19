using MiniCloudNote.Core.Entities;
using System.Threading.Tasks;

namespace MiniCloudNote.Core.Interfaces
{
    public interface INoteRepository
    {
        Task<Note> SaveAsync(Note note);
    }
}