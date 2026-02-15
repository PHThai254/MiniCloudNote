using System.ComponentModel.DataAnnotations;
namespace MiniCloudNote.Core.DTOs
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải dài ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty; 

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email {get; set; } = string.Empty;
    }
}
