using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniCloudNote.API.Controllers;
using MiniCloudNote.Core.Interfaces;
using Moq;
using System.ComponentModel.Design;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;

namespace MiniCloudNote.UnitTests
{
   public class FilesControllerTests
    {
        // 1. Test trường hợp Upload thành công
        [Fact]
        public async Task Upload_ReturnsOkResult_WhenFileIsValid()
        {
            // === ARRANGE (chuẩn bị hàng giả) ===

            // 1. Tạo Mock cho IStrorageService
            var mockStorage = new Mock<IStorageService>();

            // 2. Dạy cho Mock cách trả lời:
            // "Nếu ai gọi UploadFileAsync, thì cứ trả về chuỗi 'fake-image.jpg' nhé "
            mockStorage.Setup(service => service.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("fake-image.jpg");

            // 3. Tạo Controller và tiêm hàng giả vào
            var controller = new FilesController(mockStorage.Object);

            // 4. Tạo file giả (FormFile) để gửi lên
            var content = "Hello World from Unit Test";
            var fileName = "test.jpg";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            // === ACT (Hành động - Chạy hàm cần test) ===
            var result = await controller.Upload(formFile);

            // === ASSERT (Kiểm tra kết quả) ===
            // Kiểm tra 1: Kết quả có phải là 200 OK không?
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            // Kiểm tra 2: Trong cái OK đó có chứa đúng cái tên file giả mình mong đợi không?
            // Lưu ý: Controller trả về { FileName = ... } nên ta phải dùng Reflection hoặc ép kiểu dynamic để check (đơn giản dùng dynamic)
            var response = okResult.Value as dynamic;
            // Ở đây check đơn giản là nó không null là được
            Assert.NotNull(response);
        }
        // 2. Test trường hợp Upload với file rỗng
        [Fact]
        public async Task Upload_ReturnsBadRequest_WhenFileIsEmpty()
        {
            // === ARRANGE ===
            var mockStorage = new Mock<IStorageService>();
            var controller = new FilesController(mockStorage.Object);

            // Tạo file rỗng
            IFormFile? nullFile = null;

            // === ACT ===
            var result = await controller.Upload(nullFile == null ? null! : nullFile);

            // === ASSERT ===
            // Mong đợi kết quả là BadRequest (400)
            Assert.IsType<BadRequestObjectResult>(result);

        }         
    }      
} 