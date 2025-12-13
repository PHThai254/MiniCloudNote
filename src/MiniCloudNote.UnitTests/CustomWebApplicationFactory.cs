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
            builder.ConfigureServices(services =>
            {
                // === 1. XÓA DATABASE CŨ (Code cũ giữ nguyên) ===                
                var servicesToRemove = services.Where(d =>
                    // Tìm tất cả dịch vụ có tên chứa "DbContextOptions"
                    (d.ServiceType.Name.Contains("DbContextOptions")) || 
                    // Tìm tất cả dịch vụ có tên chứa "AppDbContext"
                    (d.ServiceType.Name.Contains("AppDbContext"))
                ).ToList();

                // Xóa không thương tiếc
                foreach (var d in servicesToRemove)
                {
                    services.Remove(d);
                }

                // === 2. (MỚI) XÓA HANGFIRE SERVER ===
                // Tìm dịch vụ chạy ngầm có tên chứa "BackgroundJobServerHostedService"
                var hangfireService = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IHostedService) && 
                         d.ImplementationType != null && 
                         d.ImplementationType.Name.Contains("BackgroundJobServerHostedService"));
                
                // Nếu tìm thấy thì xóa sổ nó đi -> Hangfire sẽ không khởi động nữa
                if (hangfireService != null) services.Remove(hangfireService);

                // === CÀI ĐẶT LẠI IN-MEMORY ===
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });

                // === KHỞI TẠO DATABASE ===
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();

                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}
