using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Auth Controller - تسجيل الدخول والخروج
/// Handles login and authentication
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;

    public AuthController(UserService userService, JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    /// <summary>
    /// تسجيل الدخول - Login
    /// POST /api/auth/login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // التحقق من بيانات الدخول
        var user = _userService.Authenticate(request.Username, request.Password);

        if (user == null)
        {
            return Unauthorized(new LoginResponse
            {
                Success = false,
                Message = "اسم المستخدم أو كلمة المرور غير صحيحة"
            });
        }

        // إنشاء التوكن
        var token = _jwtService.GenerateToken(user);

        return Ok(new LoginResponse
        {
            Success = true,
            Token = token,
            Message = "تم تسجيل الدخول بنجاح",
            User = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role,
                Email = user.Email
            }
        });
    }

    /// <summary>
    /// الحصول على معلومات المستخدم الحالي - Get Current User
    /// GET /api/auth/me
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = _userService.GetById(int.Parse(userId));

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            Email = user.Email
        });
    }

    /// <summary>
    /// الحصول على القوائم المتاحة للمستخدم - Get User Menu
    /// GET /api/auth/menu
    /// </summary>
    [HttpGet("menu")]
    [Authorize]
    public IActionResult GetUserMenu()
    {
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
        var allowedControllers = UserService.GetAllowedControllers(role);

        // بناء القائمة بناءً على الصلاحيات
        var menu = BuildMenu(allowedControllers, role);

        return Ok(menu);
    }

    /// <summary>
    /// بناء القائمة الديناميكية
    /// Build dynamic menu based on permissions
    /// </summary>
    private static List<MenuItem> BuildMenu(string[] controllers, string role)
    {
        // تعريف كل الـ Actions لكل Controller
        var controllerActions = new Dictionary<string, (string[] actions, string icon)>
        {
            ["Dashboard"] = (new[] { "Index", "Stats", "Reports", "Summary" }, "📊"),
            ["Users"] = (new[] { "List", "Details", "Create", "Edit" }, "👥"),
            ["Products"] = (new[] { "List", "Details", "Create", "Edit" }, "📦"),
            ["Orders"] = (new[] { "List", "Details", "Process", "Cancel" }, "🛒"),
            ["Settings"] = (new[] { "General", "Security", "Notifications", "Backup" }, "⚙️")
        };

        var menu = new List<MenuItem>();

        foreach (var controller in controllers)
        {
            if (controllerActions.TryGetValue(controller, out var info))
            {
                var menuItem = new MenuItem
                {
                    Controller = controller,
                    Title = GetArabicTitle(controller),
                    Icon = info.icon,
                    Actions = info.actions.Select(action => new ActionMenuItem
                    {
                        Action = action,
                        Title = GetArabicActionTitle(controller, action),
                        Url = $"/{role}/{controller.ToLower()}/{action.ToLower()}"
                    }).ToList()
                };

                menu.Add(menuItem);
            }
        }

        return menu;
    }

    private static string GetArabicTitle(string controller) => controller switch
    {
        "Dashboard" => "لوحة التحكم",
        "Users" => "المستخدمون",
        "Products" => "المنتجات",
        "Orders" => "الطلبات",
        "Settings" => "الإعدادات",
        _ => controller
    };

    private static string GetArabicActionTitle(string controller, string action) => (controller, action) switch
    {
        ("Dashboard", "Index") => "الرئيسية",
        ("Dashboard", "Stats") => "الإحصائيات",
        ("Dashboard", "Reports") => "التقارير",
        ("Dashboard", "Summary") => "الملخص",

        ("Users", "List") => "قائمة المستخدمين",
        ("Users", "Details") => "تفاصيل المستخدم",
        ("Users", "Create") => "إضافة مستخدم",
        ("Users", "Edit") => "تعديل مستخدم",

        ("Products", "List") => "قائمة المنتجات",
        ("Products", "Details") => "تفاصيل المنتج",
        ("Products", "Create") => "إضافة منتج",
        ("Products", "Edit") => "تعديل منتج",

        ("Orders", "List") => "قائمة الطلبات",
        ("Orders", "Details") => "تفاصيل الطلب",
        ("Orders", "Process") => "معالجة الطلب",
        ("Orders", "Cancel") => "إلغاء الطلب",

        ("Settings", "General") => "إعدادات عامة",
        ("Settings", "Security") => "الأمان",
        ("Settings", "Notifications") => "الإشعارات",
        ("Settings", "Backup") => "النسخ الاحتياطي",

        _ => action
    };
}

/// <summary>
/// عنصر القائمة - Menu Item
/// </summary>
public class MenuItem
{
    public string Controller { get; set; } = "";
    public string Title { get; set; } = "";
    public string Icon { get; set; } = "";
    public List<ActionMenuItem> Actions { get; set; } = new();
}

/// <summary>
/// عنصر Action في القائمة
/// </summary>
public class ActionMenuItem
{
    public string Action { get; set; } = "";
    public string Title { get; set; } = "";
    public string Url { get; set; } = "";
}
