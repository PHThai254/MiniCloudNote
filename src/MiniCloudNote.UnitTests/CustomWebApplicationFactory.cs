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
            // 1. QUAN TRỌNG NHẤT: Bật chế độ "Testing"
            // Dòng này giúp Program.cs biết là đang test để bỏ qua Redis, Hangfire...
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // 2. Chỉ cần xóa DbContext cũ (Postgres) là đủ
                // Không cần vòng lặp phức tạp nữa vì Program.cs đã tự loại bỏ Hangfire/HealthCheck rồi
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                
                // 3. Cài In-Memory Database
                services.AddDbContext<AppDbContext>(options =>
                {
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