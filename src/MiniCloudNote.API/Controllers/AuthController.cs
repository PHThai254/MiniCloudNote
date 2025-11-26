using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.API.DTOs;
using MiniCloudNote.Core.Entities;
using MiniCloudNote.Core.Interfaces;

namespace MiniCloudNote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
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
    }
}