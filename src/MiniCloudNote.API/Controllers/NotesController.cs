using Microsoft.AspNetCore.Mvc;

namespace MiniCloudNote.API.Controllers
{
    // Dữ liệu giả lập để tạo Ghi chú
    public class CreateNoteRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateNote([FromBody] CreateNoteRequest request)
        {
            // === VI PHẠM 1: Trách nhiệm xử lý Nghiệp vụ (Business Logic) ===
            // Logic nghiệp vụ: Tiêu đề không được để trống
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Tiêu đề là bắt buộc.");
            }
            // Logic nghiệp vụ: Nội dung không được quá 1000 ký tự
            if (request.Content?.Length > 1000)
            {
                return BadRequest("Nội dung quá dài.");
            }
            // === KẾT THÚC VI PHẠM 1 ===
            // === VI PHẠM 2: Trách nhiệm truy cập Database (Data Access) ===
            // Giả lập lưu vào Database
            Console.WriteLine("Đang kết nối tới PostgreSQL...");
            Console.WriteLine($"Đã lưu: Title = {request.Title}, Content = {request.Content}");
            // Giả sử đây là một câu lệnh SQL phức tạp
            // var connection = new NpgsqlConnection("...");
            // connection.Execute("INSERT INTO ...");
            // === KẾT THÚC VI PHẠM 2 ===

            // === VI PHẠM 3: Trách nhiệm gọi Dịch vụ ngoài (External Service) ===
            // Giả lập gửi email thông báo
            Console.WriteLine("Đang kết nối tới dịch vụ Email...");
            Console.WriteLine($"Gửi email tới người dùng: 'Bạn vừa tạo ghi chú {request.Title}'");
            // var smtpClient = new SmtpClient("...");
            // smtpClient.Send("...");

            return Ok("Tạo ghi chú thành công!");
            // === KẾT THÚC VI PHẠM 3 ===
        }
    }
}