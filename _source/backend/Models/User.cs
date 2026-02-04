namespace Backend.Models;

/// <summary>
/// نموذج المستخدم - User Model
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// طلب تسجيل الدخول - Login Request
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// استجابة تسجيل الدخول - Login Response
/// </summary>
public class LoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public UserInfo? User { get; set; }
}

/// <summary>
/// معلومات المستخدم المرسلة للـ Frontend
/// </summary>
public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// استجابة الـ Action - ما تعرضه كل صفحة
/// </summary>
public class ActionResponse
{
    public string UserName { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string RouteUrl { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}

/// <summary>
/// الأدوار المتاحة في النظام
/// </summary>
public static class Roles
{
    public const string Admin = "admin";
    public const string User = "user";
    public const string Editor = "editor";
    public const string Evaluator = "evaluator";

    public static readonly string[] All = { Admin, User, Editor, Evaluator };
}
