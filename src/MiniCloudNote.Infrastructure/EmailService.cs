using MiniCloudNote.Core.Interfaces; // Nhớ dòng using này để thấy Interface
using System;
using System.Threading.Tasks;

namespace MiniCloudNote.Infrastructure
{
    // Thêm ": IEmailService" vào sau tên class để thực hiện kế thừa
    public class EmailService : IEmailService
    {
        // Hàm này bắt buộc phải có vì Interface yêu cầu
        public async Task SendWelcomeEmailAsync(string email, string name)
        {
            // Logic giả lập gửi mail (Delay 5s)
            Console.WriteLine($"[Job Hangfire] Dang chuan bi gui email cho: {name} ({email})...");
            
            // Giả vờ mạng chậm mất 5 giây
            await Task.Delay(5000); 
            
            Console.WriteLine($"[Job Hangfire] -> DA GUI THANH CONG CHO: {name}!");
        }
    }
}