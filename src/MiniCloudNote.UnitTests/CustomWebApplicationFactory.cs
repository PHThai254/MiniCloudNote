using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Cần dòng này cho IHostedService
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
                // === CHIẾN DỊCH DỌN DẸP 2.0: DÙNG VÒNG LẶP QUÉT SẠCH ===
                
                // Chạy vòng lặp từ dưới lên trên để xóa an toàn (tránh lỗi Index out of range)
                for (int i = services.Count - 1; i >= 0; i--)
                {
                    var d = services[i];

                    // 1. Xóa Database cũ (Postgres)
                    if (d.ServiceType.Name.Contains("DbContextOptions") || 
                        d.ServiceType.Name.Contains("AppDbContext"))
                    {
                        services.RemoveAt(i);
                        continue;
                    }

                    // 2. Xóa Hangfire (Kẻ thù gây lỗi TaskCanceled)
                    // Hangfire chạy ngầm dưới danh nghĩa IHostedService
                    if (d.ServiceType == typeof(IHostedService))
                    {
                        // Kiểm tra tên thật của nó xem có chữ "Hangfire" hay "BackgroundJob" không
                        var implName = d.ImplementationType?.FullName ?? "";
                        if (implName.Contains("Hangfire") || implName.Contains("BackgroundJobServer"))
                        {
                            services.RemoveAt(i);
                            continue;
                        }
                    }

                    // 3. Xóa Health Checks (Kẻ thù gây lỗi 503 Service Unavailable)
                    // Các bài kiểm tra (Redis, NpgSql) được đăng ký dưới tên "HealthCheckRegistration"
                    if (d.ServiceType.Name.Contains("HealthCheckRegistration"))
                    {
                        services.RemoveAt(i);
                        continue;
                    }
                }

                // === CÀI LẠI MÔI TRƯỜNG TEST (IN-MEMORY) ===
                
                services.AddDbContext<AppDbContext>(options =>
                {
                    // Thêm Guid để mỗi lần test là một DB mới tinh, không bị cache
                    options.UseInMemoryDatabase("InMemoryDbForTesting_" + System.Guid.NewGuid());
                });

                // Khởi tạo Database
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