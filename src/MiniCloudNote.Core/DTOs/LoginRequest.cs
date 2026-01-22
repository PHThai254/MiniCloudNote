using System.ComponentModel.DataAnnotations;

namespace MiniCloudNote.Core.DTOs
{
    public class LoginRequest
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; 
 
    }
}