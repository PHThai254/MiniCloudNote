using System.Threading.Tasks;
namespace MiniCloudNote.Core.Interfaces
{
    public interface IEmailService
    {
        // Hàm gửi email tổng quát (nhận Email, Tiêu đề, Nội dung)
        // Để khớp với lệnh gọi trong AuthController
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}