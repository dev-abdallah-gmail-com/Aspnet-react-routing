using System.Text;
using Backend.Middleware;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// تسجيل الخدمات - Register Services
// ============================================

// إضافة Controllers
builder.Services.AddControllers();

// إضافة Swagger للتوثيق
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Role-Based Routing API", Version = "v1" });

    // إضافة دعم JWT في Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "أدخل التوكن بهذا الشكل: Bearer {token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// تسجيل الخدمات المخصصة - Register Custom Services
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<JwtService>();

// ============================================
// إعداد JWT Authentication
// ============================================
var jwtKey = builder.Configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyForJWTTokenGeneration123456789";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "RoleBasedRoutingDemo";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "RoleBasedRoutingDemoUsers";

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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// ============================================
// إعداد CORS للسماح بالاتصال من React
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ============================================
// Configure HTTP Request Pipeline
// ============================================

// تفعيل Swagger في بيئة التطوير
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// تفعيل CORS
app.UseCors("AllowReact");

// تفعيل Authentication و Authorization
app.UseAuthentication();
app.UseAuthorization();

// تفعيل Middleware للتحقق من الصلاحيات
app.UseRoleAuthorization();

// تسجيل الـ Controllers
app.MapControllers();

// رسالة الترحيب
app.MapGet("/", () => new
{
    Message = "مرحباً بك في Role-Based Routing API",
    Documentation = "/swagger",
    Endpoints = new
    {
        Login = "POST /api/auth/login",
        GetUser = "GET /api/auth/me",
        GetMenu = "GET /api/auth/menu"
    }
});

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════╗
║         Role-Based Routing Demo - Backend API             ║
╠═══════════════════════════════════════════════════════════╣
║  Swagger: http://localhost:5000/swagger                   ║
║  API:     http://localhost:5000/api                       ║
╠═══════════════════════════════════════════════════════════╣
║  المستخدمون المتاحون:                                     ║
║  ─────────────────────────────────────────────────────────║
║  admin     / admin123   (كل الصلاحيات)                    ║
║  editor    / editor123  (Dashboard, Products, Orders)     ║
║  evaluator / eval123    (Dashboard, Users, Orders)        ║
║  user      / user123    (Dashboard, Orders)               ║
╚═══════════════════════════════════════════════════════════╝
");

app.Run();
