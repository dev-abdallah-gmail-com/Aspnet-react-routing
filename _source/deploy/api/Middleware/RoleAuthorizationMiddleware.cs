using System.Security.Claims;
using Backend.Services;

namespace Backend.Middleware;

/// <summary>
/// Middleware للتحقق من صلاحيات الوصول بناءً على الدور
/// Role-based Authorization Middleware
/// </summary>
public class RoleAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public RoleAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // تخطي الـ endpoints التي لا تحتاج تحقق (مثل login)
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // قائمة المسارات المستثناة
        var excludedPaths = new[] { "/api/auth/login", "/swagger", "/health" };

        if (excludedPaths.Any(p => path.StartsWith(p)))
        {
            await _next(context);
            return;
        }

        // التحقق من أن المستخدم مسجل دخول
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value ?? "";

            // استخراج اسم الـ Controller من المسار
            // مثال: /api/users/list -> users
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length >= 2 && segments[0] == "api")
            {
                var controller = segments[1];

                // التحقق من الصلاحية
                if (!UserService.HasAccess(role, controller))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Error = "Forbidden",
                        Message = $"ليس لديك صلاحية للوصول إلى {controller}",
                        Role = role,
                        Controller = controller
                    });
                    return;
                }
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method لإضافة الـ Middleware
/// </summary>
public static class RoleAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRoleAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RoleAuthorizationMiddleware>();
    }
}
