using Backend.Models;

namespace Backend.Services;

/// <summary>
/// خدمة المستخدمين - تحتوي على بيانات المستخدمين الثابتة (Hardcoded)
/// User Service - Contains hardcoded user data
/// </summary>
public class UserService
{
    // قائمة المستخدمين الثابتة - Hardcoded Users
    private readonly List<User> _users = new()
    {
        // Admin - مدير النظام (كل الصلاحيات)
        new User
        {
            Id = 1,
            Username = "admin",
            Password = "admin123",
            FullName = "أحمد المدير",
            Role = Roles.Admin,
            Email = "admin@example.com"
        },
        // User - مستخدم عادي
        new User
        {
            Id = 2,
            Username = "user",
            Password = "user123",
            FullName = "محمد المستخدم",
            Role = Roles.User,
            Email = "user@example.com"
        },
        // Editor - محرر
        new User
        {
            Id = 3,
            Username = "editor",
            Password = "editor123",
            FullName = "سارة المحررة",
            Role = Roles.Editor,
            Email = "editor@example.com"
        },
        // Evaluator - مقيّم
        new User
        {
            Id = 4,
            Username = "evaluator",
            Password = "eval123",
            FullName = "خالد المقيّم",
            Role = Roles.Evaluator,
            Email = "evaluator@example.com"
        }
    };

    /// <summary>
    /// التحقق من بيانات تسجيل الدخول
    /// Authenticate user credentials
    /// </summary>
    public User? Authenticate(string username, string password)
    {
        return _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);
    }

    /// <summary>
    /// الحصول على مستخدم بواسطة الـ ID
    /// Get user by ID
    /// </summary>
    public User? GetById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    /// <summary>
    /// الحصول على مستخدم بواسطة اسم المستخدم
    /// Get user by username
    /// </summary>
    public User? GetByUsername(string username)
    {
        return _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// الحصول على كل المستخدمين
    /// Get all users
    /// </summary>
    public List<User> GetAll()
    {
        return _users;
    }

    /// <summary>
    /// التحقق من صلاحية الوصول للـ Controller
    /// Check if role has access to controller
    /// </summary>
    public static bool HasAccess(string role, string controller)
    {
        // صلاحيات كل دور - Role Permissions
        var permissions = new Dictionary<string, string[]>
        {
            // Admin - كل الصلاحيات
            [Roles.Admin] = new[] { "Dashboard", "Users", "Products", "Orders", "Settings" },

            // Editor - صلاحيات المحرر
            [Roles.Editor] = new[] { "Dashboard", "Products", "Orders" },

            // Evaluator - صلاحيات المقيّم
            [Roles.Evaluator] = new[] { "Dashboard", "Users", "Orders" },

            // User - صلاحيات المستخدم العادي
            [Roles.User] = new[] { "Dashboard", "Orders" }
        };

        if (permissions.TryGetValue(role.ToLower(), out var allowedControllers))
        {
            return allowedControllers.Contains(controller, StringComparer.OrdinalIgnoreCase);
        }

        return false;
    }

    /// <summary>
    /// الحصول على الـ Controllers المتاحة للدور
    /// Get available controllers for a role
    /// </summary>
    public static string[] GetAllowedControllers(string role)
    {
        var permissions = new Dictionary<string, string[]>
        {
            [Roles.Admin] = new[] { "Dashboard", "Users", "Products", "Orders", "Settings" },
            [Roles.Editor] = new[] { "Dashboard", "Products", "Orders" },
            [Roles.Evaluator] = new[] { "Dashboard", "Users", "Orders" },
            [Roles.User] = new[] { "Dashboard", "Orders" }
        };

        return permissions.TryGetValue(role.ToLower(), out var controllers)
            ? controllers
            : Array.Empty<string>();
    }
}
