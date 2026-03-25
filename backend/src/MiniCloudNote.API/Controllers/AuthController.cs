using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.Core.DTOs;
using MiniCloudNote.Core.Entities;
using MiniCloudNote.Core.Interfaces;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Hangfire;

namespace MiniCloudNote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, UserManager<User> userManager, IEmailService emailService)
        {
            _authService = authService;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Username,
                    FullName = request.FullName,
                    Email = request.Email 
                };

                // Đăng ký User vào DB (việc này vẫn phải chờ)
                var createdUser = await _authService.RegisterAsync(newUser, request.Password);

                // FILE-AND-FORGET: Bắn Job gửi email vào hàng đợi rồi trả về OK ngay lập tức
                // Server không cần chờ gửi mail xong mới phản hồi User -> API nhanh hơn hẳn
                BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(
                    request.Email,
                    "Welcome to MiniCloudNote",
                    $"Xin chào {request.FullName}, chúc mừng bạn đã đăng ký thành công!"
                ));
                
                return Ok(new { message = "Đăng ký thành công!", userId = createdUser.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _authService.LoginAsync(request.Username, request.Password);

            if (token == null)
            {
                return BadRequest(new { message = "INVALID_CREDENTIALS" });
            }
            return Ok(new { token = token });    
            }
            catch (Exception)
            {
                // Nếu _authService ném ra lỗi (VD: "Sai mật khẩu", "Không tìm thấy User")
                // Ta bắt lại hết và trả về mã lỗi chuẩn để Flutter tự dịch
                return BadRequest(new { message = "INVALID_CREDENTIALS" });
            }
  
        }

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // 1. Lấy ID của user đang đăng nhập từ Token
            // ClaimTypes.NameIdentifier thường chứa User ID (do Identity setup)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return BadRequest(new { message = "USER_NOT_FOUND" });
            }
            
            // 2. Tìm User trong Database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest(new { message = "USER_NOT_FOUND" });
            }

            // 3. Thực hiện đổi mật khẩu
            // Hàm này của Identity sẽ tự kiểm tra CurrentPassword có đúng luôn không
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                // Identity trả về mảng Errors. Tạm thời mình trả về mã lỗi chung chung
                return BadRequest(new { message = "CHANGE_PASSWORD_FAILED" });
            }

            return Ok(new { Message = "Đổi mật khẩu thành công!" });
        }
    }
}