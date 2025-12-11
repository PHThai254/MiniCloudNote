using MiniCloudNote.Core.Interfaces;
using MiniCloudNote.Core.Services;
using MiniCloudNote.Core.Services.FormattingStrategies;
using MiniCloudNote.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MiniCloudNote.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Amazon.S3;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// === BẮT ĐẦU: Cấu hình Redis Cache ===
// Lấy chuỗi kết nối (Ưu tiên biến môi trường nếu có, fallback về localhost)
// Khi chạy trong Docker Compose, ta sẽ set biến môi trường REDIS_CONNECTION=redis:6379
var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379";

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = "MiniCloud_"; // Tiền tố cho các key đỡ bị lẫn
});
// === KẾT THÚC ===

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MiniCloudNote API", Version = "v1" });

    // Định nghĩa bảo mật (Cái ổ khóa)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Yêu cầu bảo mật (Bắt buộc dùng ổ khóa cho các API)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Đăng ký DbContext sử dụng PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// === 1. CẤU HÌNH HANGFIRE SERVICE ===
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    // Sử dụng Database PostgreSQL để lưu Job
    .UsePostgreSqlStorage(options =>        
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")))
);

// === 2. BẬT HANGFIRE SERVER (QUAN TRỌNG) ===
// Nếu thiếu dòng này, Job sẽ chỉ nằm chờ trong DB mà không ai xử lý
builder.Services.AddHangfireServer();

// === ĐĂNG KÝ SERVICE CỦA MÌNH (DI) ===
builder.Services.AddScoped<IFormattingStrategy, MarkdownFormattingStrategy>();
builder.Services.AddScoped<IFormattingStrategy, PlainTextFormattingStrategy>();

// Đăng ký NoteService
builder.Services.AddScoped<INoteService, NoteService>();

// Đăng ký Repository (DIP - Interface map với Class)
builder.Services.AddScoped<INoteRepository, NoteRepository>();

// Đăng ký EmailService 
builder.Services.AddScoped<IEmailService, EmailService>();

// Đăng ký Authentication Services (Ngày 12)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Lấy Key từ User Secrets (Giống ngày 12)
var secretKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Jwt:Key not found!");
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

// Đăng ký dịch vụ Xác thực (Authentication)
builder.Services.AddAuthentication(options =>
{
    // Định nghĩa: Mặc định dùng JWT để xác thực
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Cấu hình máy soi vé
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // Kiểm tra vé hết hạn chưa
        ValidateIssuerSigningKey = true, // Quan trọng: Kiểm tra chữ ký (tránh làm giả)

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

// 1. Cấu hình Amazon S3 Client để trỏ về MinIO
var minioConfig = new AmazonS3Config
{
    // Ép cứng HTTP tại đây để đảm bảo không bị file cấu hình nào ghi đè
    ServiceURL = "http://localhost:9000",
    ForcePathStyle = true, // <--- BẮT BUỘC PHẢI CÓ CHO MINIO (Nếu không nó sẽ lỗi DNS)
    UseHttp = true // <--- Dùng HTTP thay vì HTTPS (MinIO thường chạy trên HTTP)
};

// 2. Đăng ký AmazonS3Client
builder.Services.AddSingleton<IAmazonS3>(sp => 
    new AmazonS3Client(
        builder.Configuration["Minio:AccessKey"], 
        builder.Configuration["Minio:SecretKey"], 
        minioConfig
    ));

// 3. Đăng ký Storage Service của mình
builder.Services.AddScoped<IStorageService, MinioStorageService>();

var app = builder.Build();

// === BẮT ĐẦU: TỰ ĐỘNG MIGRATION ===
// Tạo một phạm vi (scope) tạm thời để lấy DbContext ra dùng
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        // Lệnh này tương đương với 'dotnet ef database update'
        // Nó sẽ tự tạo Database nếu chưa có, và chạy các migration còn thiếu
        context.Database.Migrate();
        
        Console.WriteLine("--> Đã thực hiện Migration Database thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("--> Lỗi khi Migration: " + ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();


app.UseAuthentication(); // Soát vé (Bạn là ai?)
app.UseAuthorization(); // Soi quyền (Bạn được làm gì?)

// === 3. BẬT DASHBOARD ===
// Truy cập tại: /hangfire
app.UseHangfireDashboard();

// Kích hoạt các Controller (NotesController)
app.MapControllers();
app.Run();