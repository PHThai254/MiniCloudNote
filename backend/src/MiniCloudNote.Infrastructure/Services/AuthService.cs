using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniCloudNote.Core.Entities; // Namespace chứa class User
using MiniCloudNote.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity; // Cần thiết để dùng UserManager

namespace MiniCloudNote.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        // Thay IUserRepository bằng UserManager của Identity
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // 1. Đăng ký (Dùng Identity để tạo User và Hash mật khẩu chuẩn)
        public async Task<User> RegisterAsync(User user, string password)
        {

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                // Lấy lỗi đầu tiên mà Identity trả về
                var firstError = result.Errors.FirstOrDefault();
                string errorCode = "UNKNOWN_ERROR";

                if (firstError != null)
                {
                    // Map lỗi của Identity sang mã lỗi của API
                    switch (firstError.Code)
                    {
                        case "DuplicateEmail":
                            errorCode = "EMAIL_ALREADY_EXISTS";
                            break;
                        case "DuplicateUserName":
                            errorCode = "USERNAME_ALREADY_EXISTS";
                            break;
                        default:
                            // Nếu lỗi có chữ "Password" (VD: PasswordTooShort, PasswordRequiresDigit...)
                            if (firstError.Code.Contains("Password"))
                            {
                                errorCode = "WEAK_PASSWORD";
                            }
                            else
                            {
                                errorCode = "UNKNOWN_ERROR";
                            }
                            break;
                        // Thêm các lỗi khác nếu cần
                    }
                }
                // Ném MÃ LỖI (Ví dụ: "WEAK_PASSWORD") ra cho AuthController hứng
                throw new Exception(errorCode);   
            }
            return user;
        }

        // 2. Đăng nhập (Dùng Identity để kiểm tra mật khẩu)
        public async Task<string?> LoginAsync(string username, string password)
        {
            // Tìm user theo UserName
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return null;

            // Kiểm tra mật khẩu (Hàm này tự so sánh Hash của Identity)
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordValid) return null;

            // Tạo Token
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["Jwt:Key"] ?? string.Empty;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                // QUAN TRỌNG: Phải dùng ClaimTypes.NameIdentifier để khớp với 
                // User.FindFirstValue(ClaimTypes.NameIdentifier) bên Controller
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                
                // UserName
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                
                // Email (nếu cần)
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                
                // UUID cho Token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // (Tùy chọn) Nếu sau này dùng Role, lấy Role từ DB thêm vào Token
            // var roles = await _userManager.GetRolesAsync(user);
            // foreach (var role in roles)
            // {
            //     claims.Add(new Claim(ClaimTypes.Role, role));
            // }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}