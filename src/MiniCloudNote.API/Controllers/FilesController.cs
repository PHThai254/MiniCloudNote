using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.Core.Interfaces;
using MiniCloudNote.Infrastructure;
using System.Threading.Tasks;
using System;

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
                return BadRequest("File rỗng.");

            // Mở luồng đọc file
            using var stream = file.OpenReadStream();
            
            // Gọi Service xử lý
            // Hàm này giờ trả về "tên file duy nhất" (ví dụ: guid-anh.jpg)
            var fileName = await _storageService.UploadFileAsync(file.FileName, stream, file.ContentType);

            return Ok(new { FileName = fileName });
        }

        // GET: api/Files/download/guid-anh.jpg
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> GetUrl(string fileName)
        {
            // Gọi service lấy link tạm thời
            var presignedUrl = await _storageService.GetFileUrlAsync(fileName);
            return Ok(new { Url = presignedUrl });
        }

        // DELETE: api/Files/guid-anh.jpg
        [HttpDelete("{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
            await _storageService.DeleteFileAsync(fileName);
            return Ok(new { Message = "Đã xóa file thành công!" });
        }
    }
}