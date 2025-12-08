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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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
    
// === ĐĂNG KÝ SERVICE CỦA MÌNH (DI) ===
builder.Services.AddScoped<IFormattingStrategy, MarkdownFormattingStrategy>();
builder.Services.AddScoped<IFormattingStrategy, PlainTextFormattingStrategy>();

// Đăng ký NoteService
builder.Services.AddScoped<INoteService, NoteService>();

// Đăng ký Repository (DIP - Interface map với Class)
builder.Services.AddScoped<INoteRepository, NoteRepository>();

// Đăng ký EmailService (Tạm thời)
builder.Services.AddScoped<EmailService>();

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

app.UseHttpsRedirection();


app.UseAuthentication(); // Soát vé (Bạn là ai?)
app.UseAuthorization(); // Soi quyền (Bạn được làm gì?)

// Kích hoạt các Controller (NotesController)
app.MapControllers();

app.Run();