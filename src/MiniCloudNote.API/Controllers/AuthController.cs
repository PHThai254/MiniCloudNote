using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.API.DTOs;
using MiniCloudNote.Core.Entities;
using MiniCloudNote.Core.Interfaces;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Identity;

namespace MiniCloudNote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;

        public AuthController(IAuthService authService, UserManager<User> userManager)
        {
            _authService = authService;
            _userManager = userManager;
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
                    FullName = request.FullName
                };

                var createdUser = await _authService.RegisterAsync(newUser, request.Password);
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
            var token = await _authService.LoginAsync(request.Username, request.Password);

            if (token == null)
            {
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu." });
            }

            return Ok(new { token = token });
  
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
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng." });
            }
            
            // 2. Tìm User trong Database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Tài khoản không tồn tại. ");
            }

            // 3. Thực hiện đổi mật khẩu
            // Hàm này của Identity sẽ tự kiểm tra CurrentPassword có đúng luôn không
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                // Trả về lỗi (ví dụ: Sai mật khẩu cũ, mật khẩu mới không đủ mạnh....)
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "Đổi mật khẩu thành công!" });
        }
    }
}