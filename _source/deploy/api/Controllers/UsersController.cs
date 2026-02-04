using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Users Controller - إدارة المستخدمين
/// متاح لـ: Admin, Evaluator
/// </summary>
public class UsersController : BaseApiController
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// قائمة المستخدمين - List
    /// GET /api/users/list
    /// </summary>
    [HttpGet("list")]
    public IActionResult List()
    {
        var users = _userService.GetAll().Select(u => new
        {
            u.Id,
            u.Username,
            u.FullName,
            u.Role,
            u.Email
        });

        return Ok(CreateResponse("List", "قائمة جميع المستخدمين", users));
    }

    /// <summary>
    /// تفاصيل المستخدم - Details
    /// GET /api/users/details?id=1
    /// </summary>
    [HttpGet("details")]
    public IActionResult Details([FromQuery] int id = 1)
    {
        var user = _userService.GetById(id);

        if (user == null)
        {
            return NotFound(CreateResponse("Details", "المستخدم غير موجود"));
        }

        var data = new
        {
            user.Id,
            user.Username,
            user.FullName,
            user.Role,
            user.Email,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            LastLogin = DateTime.UtcNow.AddHours(-2)
        };

        return Ok(CreateResponse("Details", "تفاصيل المستخدم", data));
    }

    /// <summary>
    /// إضافة مستخدم - Create
    /// GET /api/users/create
    /// </summary>
    [HttpGet("create")]
    public IActionResult Create()
    {
        var data = new
        {
            AvailableRoles = Roles.All,
            FormFields = new[]
            {
                new { Name = "username", Type = "text", Required = true },
                new { Name = "password", Type = "password", Required = true },
                new { Name = "fullName", Type = "text", Required = true },
                new { Name = "email", Type = "email", Required = true },
                new { Name = "role", Type = "select", Required = true }
            }
        };

        return Ok(CreateResponse("Create", "صفحة إضافة مستخدم جديد", data));
    }

    /// <summary>
    /// تعديل مستخدم - Edit
    /// GET /api/users/edit?id=1
    /// </summary>
    [HttpGet("edit")]
    public IActionResult Edit([FromQuery] int id = 1)
    {
        var user = _userService.GetById(id);

        if (user == null)
        {
            return NotFound(CreateResponse("Edit", "المستخدم غير موجود"));
        }

        var data = new
        {
            User = new
            {
                user.Id,
                user.Username,
                user.FullName,
                user.Role,
                user.Email
            },
            AvailableRoles = Roles.All
        };

        return Ok(CreateResponse("Edit", "صفحة تعديل المستخدم", data));
    }
}
