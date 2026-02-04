using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Dashboard Controller - لوحة التحكم
/// متاح لـ: Admin, Editor, Evaluator, User
/// </summary>
public class DashboardController : BaseApiController
{
    /// <summary>
    /// الصفحة الرئيسية - Index
    /// GET /api/dashboard/index
    /// </summary>
    [HttpGet("index")]
    public IActionResult Index()
    {
        var data = new
        {
            TotalUsers = 150,
            TotalProducts = 450,
            TotalOrders = 1200,
            Revenue = 50000
        };

        return Ok(CreateResponse("Index", "مرحباً بك في لوحة التحكم", data));
    }

    /// <summary>
    /// الإحصائيات - Stats
    /// GET /api/dashboard/stats
    /// </summary>
    [HttpGet("stats")]
    public IActionResult Stats()
    {
        var data = new
        {
            DailyVisitors = 500,
            WeeklyVisitors = 3500,
            MonthlyVisitors = 15000,
            TopPages = new[] { "Home", "Products", "Orders" }
        };

        return Ok(CreateResponse("Stats", "إحصائيات النظام", data));
    }

    /// <summary>
    /// التقارير - Reports
    /// GET /api/dashboard/reports
    /// </summary>
    [HttpGet("reports")]
    public IActionResult Reports()
    {
        var data = new
        {
            SalesReport = new { Total = 50000, Growth = 15.5 },
            UserReport = new { NewUsers = 50, ActiveUsers = 120 },
            ProductReport = new { TopSelling = "Product A", LowStock = 5 }
        };

        return Ok(CreateResponse("Reports", "تقارير النظام", data));
    }

    /// <summary>
    /// الملخص - Summary
    /// GET /api/dashboard/summary
    /// </summary>
    [HttpGet("summary")]
    public IActionResult Summary()
    {
        var data = new
        {
            LastUpdated = DateTime.UtcNow,
            SystemStatus = "Online",
            PendingTasks = 12,
            Notifications = 5
        };

        return Ok(CreateResponse("Summary", "ملخص النظام", data));
    }
}
