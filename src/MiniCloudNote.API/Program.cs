using MiniCloudNote.Core.Interfaces;
using MiniCloudNote.Core.Services;
using MiniCloudNote.Core.Services.FormattingStrategies;
using MiniCloudNote.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MiniCloudNote.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Kích hoạt các Controller (NotesController)
app.MapControllers();

app.Run();