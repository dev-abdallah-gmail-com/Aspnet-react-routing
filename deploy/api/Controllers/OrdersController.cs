using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Orders Controller - إدارة الطلبات
/// متاح لـ: Admin, Editor, Evaluator, User
/// </summary>
public class OrdersController : BaseApiController
{
    // بيانات وهمية للطلبات - Sample orders data
    private static readonly List<object> SampleOrders = new()
    {
        new { Id = 1001, Customer = "أحمد محمد", Total = 1500, Status = "مكتمل", Date = "2024-01-15" },
        new { Id = 1002, Customer = "سارة علي", Total = 2300, Status = "قيد التنفيذ", Date = "2024-01-16" },
        new { Id = 1003, Customer = "محمود خالد", Total = 850, Status = "معلق", Date = "2024-01-17" },
        new { Id = 1004, Customer = "فاطمة أحمد", Total = 3200, Status = "مكتمل", Date = "2024-01-18" },
        new { Id = 1005, Customer = "عمر حسن", Total = 1100, Status = "ملغي", Date = "2024-01-19" }
    };

    /// <summary>
    /// قائمة الطلبات - List
    /// GET /api/orders/list
    /// </summary>
    [HttpGet("list")]
    public IActionResult List()
    {
        var data = new
        {
            Orders = SampleOrders,
            TotalCount = SampleOrders.Count,
            Statuses = new[] { "معلق", "قيد التنفيذ", "مكتمل", "ملغي" },
            Statistics = new
            {
                Pending = 1,
                Processing = 1,
                Completed = 2,
                Cancelled = 1
            }
        };

        return Ok(CreateResponse("List", "قائمة جميع الطلبات", data));
    }

    /// <summary>
    /// تفاصيل الطلب - Details
    /// GET /api/orders/details?id=1001
    /// </summary>
    [HttpGet("details")]
    public IActionResult Details([FromQuery] int id = 1001)
    {
        var data = new
        {
            Id = id,
            Customer = new
            {
                Name = "أحمد محمد",
                Email = "ahmed@example.com",
                Phone = "0501234567",
                Address = "الرياض - حي النرجس"
            },
            Items = new[]
            {
                new { ProductId = 1, Name = "لابتوب Dell", Quantity = 1, Price = 2500 },
                new { ProductId = 5, Name = "سماعات Sony", Quantity = 2, Price = 200 }
            },
            Subtotal = 2900,
            Shipping = 50,
            Tax = 435,
            Total = 3385,
            Status = "قيد التنفيذ",
            PaymentMethod = "بطاقة ائتمان",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow
        };

        return Ok(CreateResponse("Details", "تفاصيل الطلب", data));
    }

    /// <summary>
    /// معالجة الطلب - Process
    /// GET /api/orders/process?id=1001
    /// </summary>
    [HttpGet("process")]
    public IActionResult Process([FromQuery] int id = 1001)
    {
        var data = new
        {
            OrderId = id,
            CurrentStatus = "معلق",
            AvailableActions = new[]
            {
                new { Action = "approve", Label = "الموافقة على الطلب" },
                new { Action = "ship", Label = "شحن الطلب" },
                new { Action = "complete", Label = "إكمال الطلب" }
            },
            ProcessingSteps = new[]
            {
                new { Step = 1, Name = "التحقق من المخزون", Status = "مكتمل" },
                new { Step = 2, Name = "تأكيد الدفع", Status = "مكتمل" },
                new { Step = 3, Name = "التجهيز للشحن", Status = "قيد التنفيذ" },
                new { Step = 4, Name = "التسليم", Status = "معلق" }
            }
        };

        return Ok(CreateResponse("Process", "صفحة معالجة الطلب", data));
    }

    /// <summary>
    /// إلغاء الطلب - Cancel
    /// GET /api/orders/cancel?id=1001
    /// </summary>
    [HttpGet("cancel")]
    public IActionResult Cancel([FromQuery] int id = 1001)
    {
        var data = new
        {
            OrderId = id,
            OrderStatus = "قيد التنفيذ",
            CanCancel = true,
            CancellationReasons = new[]
            {
                "طلب العميل",
                "المنتج غير متوفر",
                "مشكلة في الدفع",
                "عنوان خاطئ",
                "أخرى"
            },
            RefundPolicy = "سيتم إرجاع المبلغ خلال 3-5 أيام عمل"
        };

        return Ok(CreateResponse("Cancel", "صفحة إلغاء الطلب", data));
    }
}
