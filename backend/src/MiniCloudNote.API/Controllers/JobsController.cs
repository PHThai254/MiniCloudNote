using Hangfire;
using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.Core.Interfaces;
using MiniCloudNote.Core.DTOs;


namespace MiniCloudNote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        // API này dùng để test bắn Job thủ công (nếu muốn)
        [HttpPost("welcome-email")]
        public IActionResult SendWelcomeEmail(string email, string name)
        {
            // === FIRE-AND-FORGET (Bắn và Quên) ===
            // Dòng này sẽ trả về ID của Job ngay lập tức, không chờ 5 giây
            var jobId = BackgroundJob.Enqueue<IEmailService>(x => 
            x.SendEmailAsync(
                email,
                "Welcome to MiniCloudNote",
                $"Xin chào {name}, đây là email test từ Hangfire!"
            ));

            return Ok(new JobAcceptedResponse
            { 
                Message = "Yêu cầu gửi email đã được tiếp nhận!", 
                JobId = jobId,
                Note = "Bạn không cần chờ, Job đang chạy ngầm."
            });
        }
    }
}
