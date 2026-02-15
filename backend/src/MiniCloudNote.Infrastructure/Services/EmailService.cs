using MiniCloudNote.Core.Interfaces; // Nhớ dòng using này để thấy Interface
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MiniCloudNote.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        // Hàm này bắt buộc phải có vì Interface yêu cầu
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
           // GIẢ LẬP GỬI EMAIL (Simulation)
            // Vì chúng ta chưa cấu hình SMTP thật, nên sẽ ghi log ra màn hình console.
            // Khi Hangfire chạy, bạn sẽ thấy dòng này hiện lên trong Terminal.
            
            _logger.LogInformation($"[Email Mock] Đang gửi email tới: {toEmail}");
            _logger.LogInformation($"[Email Mock] Tiêu đề: {subject}");
            _logger.LogInformation($"[Email Mock] Nội dung: {body}");

            // Giả vờ đợi 2 giây như đang gửi thật
            await Task.Delay(2000); 
            
            _logger.LogInformation($"[Email Mock] --> Đã gửi thành công tới {toEmail}");
        }
    }
}