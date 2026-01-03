using Hangfire;
using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.Core.Interfaces;
using MiniCloudNote.API.DTOs;


namespace MiniCloudNote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        // Lưu ý: Không cần Inject IEmailService vào Constructor để gọi trực tiếp
        // Hangfire sẽ tự lo việc đó lúc chạy ngầm.

        [HttpPost("welcome-email")]
        [ProducesResponseType(typeof(JobAcceptedResponse), StatusCodes.Status200OK)]
        public IActionResult SendWelcomeEmail(string email, string name)
        {
            // === FIRE-AND-FORGET (Bắn và Quên) ===
            // Dòng này sẽ trả về ID của Job ngay lập tức, không chờ 5 giây
            var jobId = BackgroundJob.Enqueue<IEmailService>(x => x.SendWelcomeEmailAsync(email, name));

            return Ok(new JobAcceptedResponse
            { 
                Message = "Yêu cầu gửi email đã được tiếp nhận!", 
                JobId = jobId,
                Note = "Bạn không cần chờ, Job đang chạy ngầm."
            });
        }
    }
}
