using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Products Controller - إدارة المنتجات
/// متاح لـ: Admin, Editor
/// </summary>
public class ProductsController : BaseApiController
{
    // بيانات وهمية للمنتجات - Sample products data
    private static readonly List<object> SampleProducts = new()
    {
        new { Id = 1, Name = "لابتوب Dell", Price = 2500, Stock = 15, Category = "إلكترونيات" },
        new { Id = 2, Name = "هاتف Samsung", Price = 1200, Stock = 30, Category = "إلكترونيات" },
        new { Id = 3, Name = "كرسي مكتب", Price = 350, Stock = 50, Category = "أثاث" },
        new { Id = 4, Name = "طاولة خشبية", Price = 500, Stock = 25, Category = "أثاث" },
        new { Id = 5, Name = "سماعات Sony", Price = 200, Stock = 100, Category = "إلكترونيات" }
    };

    /// <summary>
    /// قائمة المنتجات - List
    /// GET /api/products/list
    /// </summary>
    [HttpGet("list")]
    public IActionResult List()
    {
        var data = new
        {
            Products = SampleProducts,
            TotalCount = SampleProducts.Count,
            Categories = new[] { "إلكترونيات", "أثاث", "ملابس", "أخرى" }
        };

        return Ok(CreateResponse("List", "قائمة جميع المنتجات", data));
    }

    /// <summary>
    /// تفاصيل المنتج - Details
    /// GET /api/products/details?id=1
    /// </summary>
    [HttpGet("details")]
    public IActionResult Details([FromQuery] int id = 1)
    {
        var data = new
        {
            Id = id,
            Name = "لابتوب Dell XPS 15",
            Description = "لابتوب قوي للعمل والألعاب",
            Price = 2500,
            Stock = 15,
            Category = "إلكترونيات",
            Images = new[] { "image1.jpg", "image2.jpg" },
            Specifications = new
            {
                Processor = "Intel Core i7",
                RAM = "16GB",
                Storage = "512GB SSD"
            },
            CreatedAt = DateTime.UtcNow.AddDays(-60)
        };

        return Ok(CreateResponse("Details", "تفاصيل المنتج", data));
    }

    /// <summary>
    /// إضافة منتج - Create
    /// GET /api/products/create
    /// </summary>
    [HttpGet("create")]
    public IActionResult Create()
    {
        var data = new
        {
            Categories = new[] { "إلكترونيات", "أثاث", "ملابس", "أخرى" },
            FormFields = new[]
            {
                new { Name = "name", Type = "text", Required = true, Label = "اسم المنتج" },
                new { Name = "description", Type = "textarea", Required = true, Label = "الوصف" },
                new { Name = "price", Type = "number", Required = true, Label = "السعر" },
                new { Name = "stock", Type = "number", Required = true, Label = "الكمية" },
                new { Name = "category", Type = "select", Required = true, Label = "الفئة" },
                new { Name = "images", Type = "file", Required = false, Label = "الصور" }
            }
        };

        return Ok(CreateResponse("Create", "صفحة إضافة منتج جديد", data));
    }

    /// <summary>
    /// تعديل منتج - Edit
    /// GET /api/products/edit?id=1
    /// </summary>
    [HttpGet("edit")]
    public IActionResult Edit([FromQuery] int id = 1)
    {
        var data = new
        {
            Product = new
            {
                Id = id,
                Name = "لابتوب Dell XPS 15",
                Description = "لابتوب قوي للعمل والألعاب",
                Price = 2500,
                Stock = 15,
                Category = "إلكترونيات"
            },
            Categories = new[] { "إلكترونيات", "أثاث", "ملابس", "أخرى" }
        };

        return Ok(CreateResponse("Edit", "صفحة تعديل المنتج", data));
    }
}
