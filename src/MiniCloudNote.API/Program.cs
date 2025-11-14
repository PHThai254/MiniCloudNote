using Microsoft.OpenApi.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Services
// ----------------------

// Minimal API explorer (cho Swagger hiểu endpoint)
builder.Services.AddEndpointsApiExplorer();

// Thêm Swagger service
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MiniCloudNote API",
        Version = "v1",
        Description = "API cho dự án MiniCloudNote"
    });
});

// Thêm Controller (nếu bạn có controller)
builder.Services.AddControllers();

var app = builder.Build();

// ----------------------
// Middleware pipeline
// ----------------------

// Chỉ bật Swagger trong Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // tạo JSON swagger
    app.UseSwaggerUI();     // hiển thị UI tại /swagger
}

app.UseHttpsRedirection();

// ----------------------
// Minimal API example
// ----------------------

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )).ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Map các controller nếu có
app.MapControllers();
app.Run();

// ----------------------
// Record mẫu cho Minimal API
// ----------------------
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
