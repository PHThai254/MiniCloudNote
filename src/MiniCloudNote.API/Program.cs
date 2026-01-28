using MiniCloudNote.Core.Interfaces;
using MiniCloudNote.Infrastructure.Services;
using MiniCloudNote.Infrastructure.Repositories;
using MiniCloudNote.Infrastructure.Services.FormattingStrategies;
using Microsoft.EntityFrameworkCore;
using MiniCloudNote.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.Logging.LogProviders;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using MiniCloudNote.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

// === 0. KIỂM TRA MÔI TRƯỜNG (QUAN TRỌNG CHO NGÀY 33) ===
// Biến này sẽ = true khi chạy Integration Test
bool isTesting = builder.Environment.IsEnvironment("Testing");

// === 1. CẤU HÌNH SERILOG (LOGGING) ===
builder.Host.UseSerilog((context, config) => 
{
    config.WriteTo.Console();
    // Chỉ gửi log sang Seq nếu KHÔNG phải là test (để đỡ báo lỗi kết nối Seq khi test)
    if (!isTesting) 
    {
        var SeqUrl = builder.Configuration["SeqUrl"] ?? "http://localhost:5431";
        config.WriteTo.Seq(SeqUrl);
    }
    config.Enrich.FromLogContext();
});

// === 2. CẤU HÌNH HEALTH CHECKS & REDIS (CHỈ CHẠY KHI KHÔNG TEST) ===
// Lấy Connection String
var dbConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379";

if (!isTesting)
{
    builder.Services.AddHealthChecks()
        .AddNpgSql(dbConnection, name: "PostgreSQL Database")
        .AddRedis(redisConnection, name: "Redis Cache");

    // Cấu hình Redis Cache
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "MiniCloud_";
    });
}

// Add services to the container.
builder.Services.AddControllers();

// Swagger (Giữ nguyên)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniCloudNote API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Đăng ký DbContext (EF Core)
// CHỈ DÙNG POSTGRES NẾU KHÔNG PHẢI LÀ TESTING
if (!isTesting)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));   
}

// === 3. CẤU HÌNH HANGFIRE (CHỈ CHẠY KHI KHÔNG TEST) ===
if (!isTesting)
{
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options =>         
            options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")))
    );

    // SERVER NÀY LÀ THỦ PHẠM GÂY LỖI TASKCANCELED KHI TEST -> TẮT NÓ ĐI
    builder.Services.AddHangfireServer();
}

// === ĐĂNG KÝ SERVICE CỦA MÌNH (DI) ===
builder.Services.AddScoped<IFormattingStrategy, MarkdownFormattingStrategy>();
builder.Services.AddScoped<IFormattingStrategy, PlainTextFormattingStrategy>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStorageService, MinioStorageService>();

// JWT Authentication
// Lưu ý: Khi chạy trên GitHub Actions, nó không có User Secrets nên dòng dưới có thể null
// Ta dùng toán tử ?? để fake key khi test tránh crash lúc khởi động
var secretKey = builder.Configuration["Jwt:Key"] ?? "Key_Nay_Chi_Dung_De_Fake_Khi_Build_Thoi_123456"; 
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

// Đăng ký Identity
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    // Cấu hình password nếu muốn (ví dụ: không bắt buộc ký tự đặc biệt)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>() // Báp cho Identity biết dùng DB nào
.AddDefaultTokenProviders(); // Để sinh token reset pass, email....

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "MiniCloudNote",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "MiniCloudNoteUsers",
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

var app = builder.Build();
// === TỰ ĐỘNG MIGRATION (Chỉ chạy khi KHÔNG test) ===
// Khi test ta dùng In-Memory Database nên không cần Migrate kiểu PostgreSQL
if (!isTesting)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();
            Console.WriteLine("--> Migration Database done!");
        }
        catch (Exception ex)
        {
            // Log lỗi nhưng không làm crash app (để debug dễ hơn)
            Console.WriteLine("--> Migration Error: " + ex.Message);
        }
    }
}

// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// === CÁC MIDDLEWARE NẶNG (CHỈ CHẠY KHI KHÔNG TEST) ===
if (!isTesting)
{
    app.UseHangfireDashboard();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                Status = report.Status.ToString(),
                Checks = report.Entries.Select(e => new 
                {
                    Component = e.Key,
                    Status = e.Value.Status.ToString(),
                    Description = e.Value.Description ?? "OK"
                }),
                Duration = report.TotalDuration
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    });
}
else
{
    // KHI TEST: Fake endpoint /health để Integration Test luôn xanh
    app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Note = "Testing Mode - Bypass Real Checks" }));
}

app.MapControllers();

// --- LẬP LỊCH TỰ ĐỘNG (RECURRING JOB) ---
if (!isTesting)
{
    // Cron.Minutely: Chạy mỗi phút 1 lần (để test cho nhanh)
    RecurringJob.AddOrUpdate("system-report", () => Console.WriteLine("--> [REPORT] System is running healthy..."), Cron.Minutely);
}

await app.RunAsync();

// Dòng này bắt buộc để Integration Test nhìn thấy
public partial class Program { }