using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.Core.Interfaces;
using MiniCloudNote.Infrastructure;
using System.Threading.Tasks;

namespace MiniCloudNote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Phải đăng nhập mới được upload
    public class FilesController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public FilesController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn file.");

            // Mở luồng đọc file
            using var stream = file.OpenReadStream();
            
            // Gọi Service xử lý
            var fileUrl = await _storageService.UploadFileAsync(file.FileName, stream, file.ContentType);

            return Ok(new { Url = fileUrl });
        }
    }
}