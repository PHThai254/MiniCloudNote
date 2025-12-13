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
                // === CHIẾN THUẬT MỚI: DỌN DẸP THEO TÊN (NAME-BASED REMOVAL) ===
                // Cách này mạnh hơn vì nó bắt được cả DbContextPool, Options, Generic Options...
                
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