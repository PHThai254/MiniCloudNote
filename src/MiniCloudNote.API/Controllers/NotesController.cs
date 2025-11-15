using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.Core.Interfaces; // Thêm 1
using MiniCloudNote.Infrastructure; // Thêm 2

namespace MiniCloudNote.API.Controllers
{
    // Dữ liệu giả lập (Giữ nguyên)
    public class CreateNoteRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        // 1. Khai báo các dịch vụ (trách nhiệm đã tách)
        private readonly INoteService _noteService;
        private readonly NoteRepository _noteRepository; // Tạm thời dùng class, bài DIP sẽ dùng Interface
        private readonly EmailService _emailService;     // Tạm thời dùng class

        // 2. Tiêm (Inject) dịch vụ vào qua Constructor
        public NotesController(INoteService noteService, NoteRepository noteRepository, EmailService emailService)
        {
            _noteService = noteService;
            _noteRepository = noteRepository;
            _emailService = emailService;
        }

        [HttpPost]
        public IActionResult CreateNote([FromBody] CreateNoteRequest request)
        {
            try
            {
                // === Controller chỉ còn 1 trách nhiệm: ĐIỀU PHỐI ===

                // 1. Gọi Service để check Nghiệp vụ
                _noteService.CreateNote(request.Title, request.Content);

                // 2. Gọi Repo để lưu
                _noteRepository.Save(request.Title, request.Content);

                // 3. Gọi Email để gửi
                _emailService.SendEmail(request.Title);

                return Ok("Tạo ghi chú thành công!");
            }
            catch (ArgumentException ex) // Bắt lỗi nghiệp vụ
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex) // Bắt lỗi hệ thống
            {
                return StatusCode(500, "Lỗi hệ thống: " + ex.Message);
            }
        }
    }
}