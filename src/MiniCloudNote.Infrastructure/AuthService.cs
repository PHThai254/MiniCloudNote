using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniCloudNote.Core.Entities;
using MiniCloudNote.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiniCloudNote.Infrastructure
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration; // Để đọc Secret Key từ appsettings

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        // 1. Đăng ký: Hash mật khẩu -> Lưu DB
        public async Task<User> RegisterAsync(User user, string password)
        {
            // Kiểm tra trùng username (nên làm)
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null) throw new Exception("Username đã tồn tại!");

            // Hash mật khẩu
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            
            await _userRepository.AddAsync(user);
            return user;
        }

        // 2. Đăng nhập: Kiểm tra Hash -> Tạo Token
        public async Task<string?> LoginAsync(string username, string password)
        {
            // Tìm user
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null; // Hoặc throw exception

            // Kiểm tra mật khẩu (So sánh Hash)
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isValid) return null;

            // Tạo JWT Token (Cấp vòng tay)
            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["Jwt:Key"] ?? string.Empty; // Lấy từ User Secrets
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // ID người dùng
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim("role", user.Role) // Quyền
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Hết hạn sau 1 giờ
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}