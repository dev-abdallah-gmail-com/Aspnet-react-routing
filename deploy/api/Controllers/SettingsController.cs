using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// Settings Controller - إعدادات النظام
/// متاح لـ: Admin فقط
/// </summary>
public class SettingsController : BaseApiController
{
    /// <summary>
    /// الإعدادات العامة - General
    /// GET /api/settings/general
    /// </summary>
    [HttpGet("general")]
    public IActionResult General()
    {
        var data = new
        {
            SiteName = "متجر إلكتروني",
            SiteDescription = "أفضل متجر للتسوق الإلكتروني",
            DefaultLanguage = "ar",
            AvailableLanguages = new[] { "ar", "en" },
            Timezone = "Asia/Riyadh",
            Currency = "SAR",
            DateFormat = "dd/MM/yyyy",
            MaintenanceMode = false
        };

        return Ok(CreateResponse("General", "الإعدادات العامة للنظام", data));
    }

    /// <summary>
    /// إعدادات الأمان - Security
    /// GET /api/settings/security
    /// </summary>
    [HttpGet("security")]
    public IActionResult Security()
    {
        var data = new
        {
            PasswordPolicy = new
            {
                MinLength = 8,
                RequireUppercase = true,
                RequireLowercase = true,
                RequireDigit = true,
                RequireSpecialChar = true
            },
            SessionTimeout = 30,
            MaxLoginAttempts = 5,
            TwoFactorEnabled = false,
            IpWhitelist = new[] { "192.168.1.0/24" },
            LastSecurityAudit = DateTime.UtcNow.AddDays(-7)
        };

        return Ok(CreateResponse("Security", "إعدادات الأمان", data));
    }

    /// <summary>
    /// إعدادات الإشعارات - Notifications
    /// GET /api/settings/notifications
    /// </summary>
    [HttpGet("notifications")]
    public IActionResult Notifications()
    {
        var data = new
        {
            EmailNotifications = new
            {
                Enabled = true,
                NewOrder = true,
                OrderStatusChange = true,
                NewUser = true,
                LowStock = true
            },
            PushNotifications = new
            {
                Enabled = false,
                NewOrder = false,
                OrderStatusChange = false
            },
            SMSNotifications = new
            {
                Enabled = true,
                Provider = "Twilio",
                NewOrder = true
            },
            EmailTemplates = new[]
            {
                new { Name = "welcome", Subject = "مرحباً بك" },
                new { Name = "order_confirmation", Subject = "تأكيد الطلب" },
                new { Name = "order_shipped", Subject = "تم شحن طلبك" }
            }
        };

        return Ok(CreateResponse("Notifications", "إعدادات الإشعارات", data));
    }

    /// <summary>
    /// النسخ الاحتياطي - Backup
    /// GET /api/settings/backup
    /// </summary>
    [HttpGet("backup")]
    public IActionResult Backup()
    {
        var data = new
        {
            AutoBackup = new
            {
                Enabled = true,
                Frequency = "daily",
                Time = "03:00",
                RetentionDays = 30
            },
            LastBackup = new
            {
                Date = DateTime.UtcNow.AddHours(-8),
                Size = "256 MB",
                Status = "ناجح"
            },
            BackupHistory = new[]
            {
                new { Date = DateTime.UtcNow.AddDays(-1), Size = "255 MB", Status = "ناجح" },
                new { Date = DateTime.UtcNow.AddDays(-2), Size = "254 MB", Status = "ناجح" },
                new { Date = DateTime.UtcNow.AddDays(-3), Size = "253 MB", Status = "ناجح" }
            },
            StorageLocation = "/backups",
            AvailableSpace = "50 GB"
        };

        return Ok(CreateResponse("Backup", "إعدادات النسخ الاحتياطي", data));
    }
}
