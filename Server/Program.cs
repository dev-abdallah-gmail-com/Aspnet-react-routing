using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

// قاعدة بيانات SQLite محلية
builder.Services.AddDbContext<CensusDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Census")
                      ?? "Data Source=census.db"));

// خدمة الذكاء الصناعي (Claude)
builder.Services.AddSingleton<ClaudeService>();

builder.Services.AddControllers().AddJsonOptions(o =>
{
    // تفادي دورات المراجع عند تسلسل الكيانات المترابطة
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// السماح لخادم Vite أثناء التطوير
builder.Services.AddCors(options =>
    options.AddPolicy("client", p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

// تهيئة قاعدة البيانات عند الإقلاع
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CensusDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("client");
app.MapControllers();

app.Run("http://localhost:5080");
