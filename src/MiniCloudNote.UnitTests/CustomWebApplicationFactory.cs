using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Để dùng IHostedService
using MiniCloudNote.Infrastructure.Data;
using System.Linq;

namespace MiniCloudNote.UnitTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // === CHIẾN DỊCH: DỌN SẠCH BÁCH MỌI THỨ CẢN ĐƯỜNG ===

                // 1. Tìm tất cả các dịch vụ "gây rắc rối" bằng cách soi tên (Name-based search)
                // Cách này bất bại vì không cần quan tâm Type cụ thể nằm ở đâu
                var servicesToRemove = services.Where(d =>
                    // a. Xóa DbContext cũ (Cấu hình Npgsql)
                    d.ServiceType.Name.Contains("DbContextOptions") ||
                    d.ServiceType.Name.Contains("AppDbContext") ||
                    
                    // b. Xóa Hangfire (Server chạy ngầm)
                    d.ImplementationType?.Name.Contains("BackgroundJobServerHostedService") == true ||
                    d.ServiceType.Name.Contains("IHostedService") && d.ImplementationType?.Name.Contains("Hangfire") == true ||

                    // c. Xóa Health Checks (Redis, NpgSql...) 
                    // Health Check đăng ký các "HealthCheckRegistration" vào DI. Ta xóa hết đi để nó không check nữa.
                    d.ServiceType.Name.Contains("HealthCheckRegistration")
                ).ToList();

                // 2. Xóa sổ chúng
                foreach (var d in servicesToRemove)
                {
                    services.Remove(d);
                }

                // === THIẾT LẬP LẠI MÔI TRƯỜNG TEST ===

                // 3. Cài Database In-Memory (RAM)
                services.AddDbContext<AppDbContext>(options =>
                {
                    // Đặt tên DB khác nhau cho mỗi lần chạy để tránh cache
                    options.UseInMemoryDatabase("InMemoryDbForTesting_" + System.Guid.NewGuid());
                });

                // 4. Khởi tạo Database
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();

                    db.Database.EnsureCreated();
                }
            });
        }
    }
}