using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.Core.DTOs; // <-- Đã trỏ đúng về Core
using MiniCloudNote.Core.Interfaces;
using System.Security.Claims;

namespace MiniCloudNote.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Bắt buộc phải đăng nhập mới dùng được Controller này
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly IStorageService _storageService;

        public NotesController(INoteService noteService, IStorageService storageService)
        {
            _noteService = noteService;
            _storageService = storageService;
        }

        // --- HÀM TIỆN ÍCH (HELPER METHOD) ---
        // Lấy User ID từ Token của người đang đăng nhập
        private Guid GetUserId()
        {
            // ClaimTypes.NameIdentifier chính là cái chúng ta đã nhét vào Token lúc Login
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (Guid.TryParse(userIdString, out var userId))
            {
                return userId;
            }
            // Nếu không lấy được ID (Token lỗi hoặc cũ), ném lỗi 401
            throw new UnauthorizedAccessException("User ID không hợp lệ.");
        }

        // 1. Lấy danh sách ghi chú của tôi
        // GET: api/Notes?pageIndex=1&pageSize=10&searchTerm=abc&sortBy=created_desc
        [HttpGet]
        public async Task<IActionResult> GetMyNotes([FromQuery] NoteQueryParameters query)
        {
            var userId = GetUserId();
            var notes = await _noteService.GetUserNotesAsync(userId, query);
            return Ok(notes);
        }

        // 2. Lấy chi tiết 1 ghi chú
        // GET: api/Notes/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote(Guid id)
        {
            var userId = GetUserId();
            var note = await _noteService.GetNoteByIdAsync(id, userId);
            
            if (note == null) return NotFound(new { message = "Ghi chú không tồn tại hoặc bạn không có quyền truy cập." });
            
            return Ok(note);
        }

        // 3. Tạo ghi chú mới
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            // Truyền UserId vào để Service biết ai là chủ
            var createdNote = await _noteService.CreateNoteAsync(userId, request);

            // Trả về 201 Created kèm theo Link để xem chi tiết ghi chú vừa tạo
            return CreatedAtAction(nameof(GetNote), new { id = createdNote.Id }, createdNote);
        }

        // 4. Cập nhật ghi chú
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(Guid id, [FromBody] UpdateNoteRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var isUpdated = await _noteService.UpdateNoteAsync(id, userId, request);

            if (!isUpdated) return NotFound(new { message = "Không tìm thấy ghi chú để cập nhật." });

            return Ok(new { message = "Cập nhật thành công!" });
        }

        // 5. Xóa ghi chú
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            var userId = GetUserId();
            var isDeleted = await _noteService.DeleteNoteAsync(id, userId);

            if (!isDeleted) return NotFound(new { message = "Không tìm thấy ghi chú để xóa." });

            return Ok(new { message = "Xóa thành công!" });
        }
        // 6. Upload file
        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // Bắt buộc dòng này để nhận file
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
        {
            // Lấy ruột ra
            var file = request.File; 

            // 1. Kiểm tra rỗng
            if (file == null || file.Length == 0) 
                return BadRequest("Vui lòng chọn file để upload.");

            // === NÂNG CẤP BẢO MẬT: KIỂM TRA ĐUÔI FILE ===
            // Lấy đuôi file và chuyển về chữ thường (ví dụ: .JPG -> .jpg)
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            
            // Danh sách đuôi cho phép
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif).");
            }
            // ===============================================

            // 2. Kiểm tra MIME Type (Lớp bảo vệ thứ 2)
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Nội dung file không phải là hình ảnh hợp lệ.");

            // 3. Kiểm tra dung lượng (5MB)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("File quá lớn (tối đa 5MB).");

            try
            {
                // 2. Mở Stream để đọc file
                using var stream = file.OpenReadStream();

                // 3. Gọi Service đẩy lên MinIO
                var fileName = await _storageService.UploadFileAsync(file.FileName, stream, file.ContentType);

                // 4. Trả về đường dẫn (hoặc tên file) cho Client
                // Client sẽ dùng tên này để nhét vào field "Content" của Note (ví dụ: ![img](fileName))
                return Ok(new { FileName = fileName, Message = "Upload thành công!" });      
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi upload: {ex.Message}");
            }
        }
    }
    // Class dùng để hứng dữ liệu upload
    public class UploadFileRequest
    {
        public IFormFile? File { get; set; } 
    }
}