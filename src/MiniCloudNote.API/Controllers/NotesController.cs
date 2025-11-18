using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.Core.Interfaces; 
using MiniCloudNote.Infrastructure; 
using MiniCloudNote.API.DTOs; 
using MiniCloudNote.Core.Entities; //Thêm Entity (để Mapping)
using System.Threading.Tasks; // Thêm Async

namespace MiniCloudNote.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        // 1. Khai báo các dịch vụ (trách nhiệm đã tách)
        private readonly INoteService _noteService;
        // Bỏ Repository và Email, Controller không cần biết chúng (SRP)
        //private readonly NoteRepository _noteRepository; // Tạm thời dùng class, bài DIP sẽ dùng Interface
        //private readonly EmailService _emailService;     // Tạm thời dùng class

        // 2. Tiêm (Inject) dịch vụ vào qua Constructor
        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
          
        }

        // 3. Sửa lại hàm CreateNote để dùng Service
        [HttpPost]
        public async Task<IActionResult> CreateNote([FromBody] CreateNoteRequest request)
        {
            try
            {
                // === Controller chỉ còn 1 trách nhiệm: ĐIỀU PHỐI ===

                // 1. Gọi Service (chỉ truyền dữ liệu thô)
                var newNoteEntity = await _noteService.CreateNoteAsync(request.Title, request.Content);

                // 2. Mapping: Chuyển đổi Entity -> DTO Response
                var responseDto = new NoteResponse
                {
                    Id = newNoteEntity.Id,
                    Title = newNoteEntity.Title,
                    Content = newNoteEntity.Content,
                    CreatedAt = newNoteEntity.CreatedAt
                };

                // 3. Trả về 201 Created (chuẩn REST)
                // CreatedAtAction sẽ trả về URL của tài nguyên mới trong Header
                return CreatedAtAction(nameof(GetNoteById), new { id = responseDto.Id }, responseDto);
                
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

        // Hàm giả lập cho CreatedAtAction
        [HttpGet("{id}")]
        public IActionResult GetNoteById(Guid id)
        {
            return Ok($"Đang lấy note {id}");
        }
        
        [HttpPost("format")]
        public IActionResult FormatNote([FromBody] FormatNoteRequest request)
        {
            // Controller gọi Service (tuân thủ SRP)
            var formattedContent = _noteService.FormatNoteContent(request.Content, request.FormatType);
            return Ok(formattedContent);
        }
    }
}