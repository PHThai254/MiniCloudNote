using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MiniCloudNote.Core.Entities
{
    // Phải kế thừa IdentityUser<Guid> nếu dùng Guid làm ID
    // Kế thừa IdentityUser<Guid> nghĩa là nó đã tự có: Id (Guid), UserName, Email, PasswordHash...
    public class User : IdentityUser<Guid>
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; } = "User"; // Mặc định là User thường
    }
}