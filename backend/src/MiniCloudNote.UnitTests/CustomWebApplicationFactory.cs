using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniCloudNote.Infrastructure.Data;
using System.Linq;

namespace MiniCloudNote.UnitTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Đặt môi trường là Testing để Program.cs nhận biết
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Dù Program.cs đã chặn Npgsql, ta vẫn quét dọn 1 lần nữa cho chắc chắn (Double-check)
                // Xóa tất cả Service liên quan đến DbContextOptions (Kẻ thù gây lỗi "Single database provider")
                var optionsDescriptors = services.Where(d => 
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) || 
                    d.ServiceType == typeof(DbContextOptions)
                ).ToList();

                foreach (var d in optionsDescriptors)
                {
                    services.Remove(d);
                }

                // Cài đặt In-Memory Database
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting_" + System.Guid.NewGuid());
                });

                // Khởi tạo Database
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}