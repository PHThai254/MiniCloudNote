namespace MiniCloudNote.Infrastructure
{
    public class EmailService // Sẽ implement IEmailService ở bài DIP
    {
        public void SendEmail(string title)
        {
            // === TRÁCH NHIỆM 3: Email (đã chuyển về đây) ===
            Console.WriteLine("Đang kết nối tới dịch vụ Email...");
            Console.WriteLine($"Gửi email tới người dùng: 'Bạn vừa tạo ghi chú {title}'");
        }
    }
}