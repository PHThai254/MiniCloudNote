using System.ComponentModel.DataAnnotations;

namespace MiniCloudNote.Core.DTOs
{
    public class ChangePasswordRequest
    {
        [Required]
        public required string CurrentPassword { get; set;}

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
        public required string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        public required string ConfirmNewPassword { get; set; }
    }
}