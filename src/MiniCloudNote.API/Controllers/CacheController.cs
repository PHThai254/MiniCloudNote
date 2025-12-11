using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed; // Thư viện chuẩn của .NET để dùng Cache
using System.Text;
using System.Threading.Tasks;

namespace MiniCloudNote.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IDistributedCache _cache;

        // Inject IDistributedCache vào (Nó sẽ tự dùng Redis nhờ cấu hình ở Program.cs)
        public CacheController(IDistributedCache cache)
        {
            _cache = cache;
        }

        // 1. Ghi dữ liệu vào Redis
        [HttpPost("set")]
        public async Task<IActionResult> SetCache(string key, string value)
        {
            // Chuyển string sang byte (Redis lưu dạng binary) - Hoặc dùng Extension SetStringAsync
            await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) // Cache sống 10 phút
            });

            return Ok($"Đã lưu vào Redis: {key} = {value}");
        }

        // 2. Đọc dữ liệu từ Redis
        [HttpGet("get/{key}")]
        public async Task<IActionResult> GetCache(string key)
        {
            var value = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(value))
            {
                return NotFound("Không tìm thấy hoặc Cache đã hết hạn!");
            }

            return Ok(new { Key = key, Value = value, Source = "Redis Cache 🚀" });
        }
    }
}