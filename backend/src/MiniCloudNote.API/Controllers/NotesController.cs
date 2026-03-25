using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.Core.DTOs; // <-- Đã trỏ đúng về Core
using MiniCloudNote.Core.Interfaces;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Text;
using System.Text.Json; // Để dùng IEnumerable

namespace MiniCloudNote.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Bắt buộc phải đăng nhập mới dùng được Controller này
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly IStorageService _storageService;
        private readonly IDistributedCache _cache;

        public NotesController(INoteService noteService, IStorageService storageService, IDistributedCache cache)
        {
            _noteService = noteService;
            _storageService = storageService;
            _cache = cache;
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

        // Lấy danh sách ghi chú của tôi
        // GET: api/Notes?pageIndex=1&pageSize=10&searchTerm=abc&sortBy=created_desc
        [HttpGet]
        public async Task<IActionResult> GetMyNotes([FromQuery] NoteQueryParameters query)
        {
            var userId = GetUserId();
            // TẠO CACHE KEY ĐỘC NHẤT
            // Key phải bao gồm UserID + Page + Search + Sort để tránh nhầm lẫn giữa các trang
            // Ví dụ key: "notes:guid-123:search-abc:sort-date:p-1:s-10"
            string cacheKey = $"note:{userId}:{query.SearchTerm}:{query.SortBy}:{query.PageIndex}:{query.PageSize}";
            // KIỂM TRA REDIS
            try
            {
                var cachedData = await _cache.GetAsync(cacheKey);
                if (cachedData != null)
                {
                    // HIT CACHE: Có dữ liệu trong RAM -> Trả về luôn (Siêu nhanh)
                    var jsonString = Encoding.UTF8.GetString(cachedData);

                    // Lưu ý: Deserialize đúng kiểu dữ liệu trả về của Service (thường là IEnumerable<NoteDto> hoặc PagedResult)
                    var cachedNotes = JsonSerializer.Deserialize<IEnumerable<NoteResponse>>(jsonString);
                    return Ok(cachedNotes);
                }
            }
            catch (Exception ex)
            {
                // Nếu Redis chết, log lỗi nhưng KHÔNG ĐƯỢC làm sập App
                // Vẫn cho chạy tiếp xuống DB để lấy dữ liệu (Fallback)
                Console.WriteLine($"--> Redis Error: {ex.Message}");
            }

            // MISS CACHE -> GỌI DATABASE
            var notes = await _noteService.GetUserNotesAsync(userId, query);

            // LƯU VÀO REDIS (SỐNG 60 GIÂY)
            try
            {
                var cacheOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(60)) // Hết hạn cứng sau 60s
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30)); // Nếu có người xem liên tục thì gia hạn thêm 30s

                var jsonToCache = JsonSerializer.Serialize(notes);
                await _cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(jsonToCache), cacheOptions);
            }
            catch
            {
                // Lỗi lưu cache thì bỏ qua
            }
            return Ok(notes);
        }

        // Lấy chi tiết 1 ghi chú
        // GET: api/Notes/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote(Guid id)
        {
            var userId = GetUserId();
            var note = await _noteService.GetNoteByIdAsync(id, userId);
            
            if (note == null) return NotFound(new { message = "Ghi chú không tồn tại hoặc bạn không có quyền truy cập." });
            
            return Ok(note);
        }

        // Tạo ghi chú mới
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

        // Cập nhật ghi chú
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(Guid id, [FromBody] UpdateNoteRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = GetUserId();
            var isUpdated = await _noteService.UpdateNoteAsync(id, userId, request);

            if (!isUpdated) return NotFound(new { message = "Không tìm thấy ghi chú để cập nhật." });

            return Ok(new { message = "Cập nhật thành công!" });
        }

        // Xóa ghi chú
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            var userId = GetUserId();
            var isDeleted = await _noteService.DeleteNoteAsync(id, userId);

            if (!isDeleted) return NotFound(new { message = "Không tìm thấy ghi chú để xóa." });

            return Ok(new { message = "Xóa thành công!" });
        }

        // KHU VỰC THÙNG RÁC (TRASH BIN)
        
        // Lấy danh sách ghi chú trong Thùng rác
        // GET: api/Notes/trash?pageIndex=1&pageSize=10
        [HttpGet("trash")]
        public async Task<IActionResult> GetTrashNotes([FromQuery] NoteQueryParameters query)
        {
            var userId = GetUserId();
            // viết thêm 1 hàm GetTrashNotesAsync bên Service và Repository 
            // với điều kiện lọc IsDeleted == true.
            
            var trashNotes = await _noteService.GetTrashNotesAsync(userId, query);
            return Ok(trashNotes); // Trả về danh sách rác thật!
        }

        // Phục hồi ghi chú
        // PUT: api/Notes/{id}/restore
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreNote(Guid id)
        {
            var userId = GetUserId();
            var isRestored = await _noteService.RestoreNoteAsync(id, userId);

            if (!isRestored) 
                return NotFound(new { message = "Không tìm thấy ghi chú trong thùng rác hoặc bạn không có quyền." });

            return Ok(new { message = "Đã phục hồi ghi chú thành công!" });
        }

        // Xóa vĩnh viễn ghi chú
        // DELETE: api/Notes/{id}/hard
        [HttpDelete("{id}/hard")]
        public async Task<IActionResult> HardDeleteNote(Guid id)
        {
            var userId = GetUserId();
            var isDeleted = await _noteService.HardDeleteNoteAsync(id, userId);

            if (!isDeleted) 
                return NotFound(new { message = "Không tìm thấy ghi chú để xóa vĩnh viễn." });

            return Ok(new { message = "Đã xóa vĩnh viễn ghi chú khỏi hệ thống." });
        }

        // Upload file
        [HttpPost("upload")]
        [Consumes("multipart/form-data")] // Bắt buộc dòng này để nhận file
        public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
        {
            // Lấy ruột ra
            var file = request.File; 

            // Kiểm tra rỗng
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

            // Kiểm tra MIME Type (Lớp bảo vệ thứ 2)
            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Nội dung file không phải là hình ảnh hợp lệ.");

            // Kiểm tra dung lượng (5MB)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("File quá lớn (tối đa 5MB).");

            try
            {
                // Mở Stream để đọc file
                using var stream = file.OpenReadStream();

                // Gọi Service đẩy lên MinIO
                var fileName = await _storageService.UploadFileAsync(file.FileName, stream, file.ContentType);

                // Trả về đường dẫn (hoặc tên file) cho Client
                // Client sẽ dùng tên này để nhét vào field "Content" của Note (ví dụ: ![img](fileName))
                return Ok(new { FileName = fileName, Message = "Upload thành công!" });      
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi upload: {ex.Message}");
            }
        }
        // Hàm để Frontend gọi vào xin link
        // GET: api/Notes/file/ten-file-dai-ngoang.jpg
        [HttpGet("file/{fileName}")]
        public async Task<IActionResult> GetFileUrl(string fileName)
        {
            try
            {
                // Gọi Service lấy link
                var url = await _storageService.GetFileUrlAsync(fileName);

                // Trả về link cho Client
                return Ok(new { Url = url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi lấy link ảnh: {ex.Message}");
            }
        }
    }
    // Class dùng để hứng dữ liệu upload
    public class UploadFileRequest
    {
        public IFormFile? File { get; set; } 
    }
}