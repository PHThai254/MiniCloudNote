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

        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
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
    }
}