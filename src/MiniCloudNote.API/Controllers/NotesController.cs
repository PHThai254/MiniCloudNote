using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.Core.Interfaces; 
using MiniCloudNote.Infrastructure; 
using MiniCloudNote.API.DTOs; 
using MiniCloudNote.Core.Entities; //Thêm Entity (để Mapping)
using System.Threading.Tasks; // Thêm Async
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace MiniCloudNote.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // <--- Dán cái bùa này vào là khóa toàn bộ API trong Controller này
    public class NotesController : ControllerBase
    {
        // 1. Khai báo các dịch vụ (trách nhiệm đã tách)
        private readonly INoteService _noteService;
        private readonly IConfiguration _configuration;
        // Bỏ Repository và Email, Controller không cần biết chúng (SRP)
        //private readonly NoteRepository _noteRepository; // Tạm thời dùng class, bài DIP sẽ dùng Interface
        //private readonly EmailService _emailService;     // Tạm thời dùng class

        // 2. Tiêm (Inject) dịch vụ vào qua Constructor
        public NotesController(INoteService noteService, IConfiguration configuration)
        {
            _noteService = noteService;
            _configuration = configuration;
          
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

                // 3. Đổi GetNoteById thành GetById (Tên của hàm GET ở dưới)
                return CreatedAtAction(nameof(GetById), new { id = responseDto.Id }, responseDto);
                
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

        // 1. GET: api/Notes (Lấy danh sách)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            Console.WriteLine("--- TEST HOT RELOAD ---");
            var notes = await _noteService.GetAllNotesAsync();
            return Ok(notes);
        }

        // 2. GET: api/Notes/{id} (Lấy 1 cái)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null) return NotFound("Không tìm thấy ghi chú.");
            return Ok(note);
        }

        // 3. PUT: api/Notes/{id} (Sửa)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateNoteRequest request)
        {
            try 
            {
                await _noteService.UpdateNoteAsync(id, request.Title, request.Content);
                return NoContent(); // 204 No Content (Chuẩn khi update thành công)
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tìm thấy ghi chú để sửa.");
            }
        }

        // 4. DELETE: api/Notes/{id} (Xóa)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _noteService.DeleteNoteAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tìm thấy ghi chú để xóa.");
            }
        }
        
        [HttpPost("format")]
        public IActionResult FormatNote([FromBody] FormatNoteRequest request)
        {
            // Controller gọi Service (tuân thủ SRP)
            var formattedContent = _noteService.FormatNoteContent(request.Content, request.FormatType);
            return Ok(formattedContent);
        }

        // 4. Thêm API kiểm tra cấu hình (Test nhanh)
        [HttpGet("config-test")]
        public IActionResult GetConfig()
        {
            // Đọc giá trị "MyName" từ file json
            var myName = _configuration["MyName"];

            // Đọc chuỗi kết nối (để xem nó có lấy đúng từ User Secrets không)
            var connStr = _configuration.GetConnectionString("DefaultConnection");

            return Ok(new
            {
                EnvironmentName = myName + " - Test Override Day 19", 
                ConnectionString = connStr
            });
        }

        // 5. Thêm API trả vể tên server (Test nhanh)
        [HttpGet("who-am-i")]
        [AllowAnonymous] // Cho phép ai cũng gọi được để test cho nhanh
        public IActionResult WhoAmI()
        {
            // Lấy tên máy (Trong Docker nó là Container ID)
            var serverName = Environment.MachineName;
    
            return Ok(new { 
                Message = "Xin chào! Tôi là nhân viên phục vụ bạn.",
                ServerId = serverName 
            });
        }
    }
}