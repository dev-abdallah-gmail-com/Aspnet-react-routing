using System.Security.Claims;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Base Controller - الـ Controller الأساسي الذي يرث منه كل الـ Controllers
/// يحتوي على الدوال المشتركة
/// </summary>
[ApiController]
[Authorize] // يتطلب تسجيل دخول لكل الـ Actions
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// الحصول على معلومات المستخدم الحالي من الـ JWT Token
    /// Get current user info from JWT Token
    /// </summary>
    protected UserInfo GetCurrentUser()
    {
        return new UserInfo
        {
            Id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
            Username = User.FindFirst(ClaimTypes.Name)?.Value ?? "",
            FullName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? "",
            Role = User.FindFirst(ClaimTypes.Role)?.Value ?? "",
            Email = User.FindFirst(ClaimTypes.Email)?.Value ?? ""
        };
    }

    /// <summary>
    /// إنشاء ActionResponse موحد لكل الـ Actions
    /// Create standardized ActionResponse for all actions
    /// </summary>
    protected ActionResponse CreateResponse(string action, string message, object? data = null)
    {
        var user = GetCurrentUser();
        var controller = GetType().Name.Replace("Controller", "");

        return new ActionResponse
        {
            UserName = user.FullName,
            UserRole = user.Role,
            Controller = controller,
            Action = action,
            // الـ URL يتبع النمط: /{role}/{controller}/{action}
            RouteUrl = $"/{user.Role}/{controller.ToLower()}/{action.ToLower()}",
            Message = message,
            Data = data
        };
    }
}
