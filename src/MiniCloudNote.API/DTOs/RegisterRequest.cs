using System.ComponentModel.DataAnnotations;
namespace MiniCloudNote.API.DTOs
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; 

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
    }
}
