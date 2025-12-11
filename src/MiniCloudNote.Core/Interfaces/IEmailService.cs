using System.Threading.Tasks;
namespace MiniCloudNote.Core.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string email, string name);
    }
}