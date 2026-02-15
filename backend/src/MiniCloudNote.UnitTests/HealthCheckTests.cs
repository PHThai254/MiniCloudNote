using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Xunit;

namespace MiniCloudNote.UnitTests
{
    // Kế thừa IClassFixture để xUnit tự động bật/tắt Server ảo
    public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HealthCheckTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task HealthCheck_ReturnsHealthy()
        {
            // Arrange
            // Tạo một HttpClient ảo (giống như mở Postman)
            var client = _factory.CreateClient();

            // Act
            // Gọi thử vào đường dẫn /health
            var response = await client.GetAsync("/health");

            // Assert
            // 1. Kiểm tra HTTP Status có phải 200 OK không?
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            // 2. Đọc nội dung trả về
            var responseString = await response.Content.ReadAsStringAsync();

            // 3. Kiểm tra xem có chữ "Healthy" trong đó không
            Assert.Contains("Healthy", responseString);
        }
    }
}