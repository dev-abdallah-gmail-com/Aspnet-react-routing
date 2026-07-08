using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/households")]
public class HouseholdsController : ControllerBase
{
    private readonly CensusDbContext _db;

    public HouseholdsController(CensusDbContext db) => _db = db;

    // حفظ الأسرة كاملة: رب الأسرة والأفراد (جدول Person واحد) + سجل العمليات
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SaveHouseholdRequest req)
    {
        if (req.Persons.Count == 0)
            return BadRequest(new { error = "لا يوجد أفراد لحفظهم" });

        // ضمان وجود رب أسرة واحد بالضبط
        var heads = req.Persons.Count(p => p.IsHead);
        if (heads != 1)
            return BadRequest(new { error = "يجب أن يكون هناك رب أسرة واحد بالضبط (IsHead)" });

        var household = new Household
        {
            PhoneNumber = req.PhoneNumber,
            MemberCount = req.MemberCount,
            Persons = req.Persons.Select(p => new Person
            {
                Name = p.Name,
                BirthDate = p.BirthDate,
                Gender = p.Gender,
                Occupation = p.Occupation,
                Nationality = p.Nationality,
                IdNumber = p.IdNumber,
                IdExpiryDate = p.IdExpiryDate,
                Relationship = p.Relationship,
                IsHead = p.IsHead,
            }).ToList(),
            Logs = req.Logs.Select(l => new ActivityLog
            {
                PersonLabel = l.PersonLabel,
                FieldName = l.FieldName,
                OldValue = l.OldValue,
                NewValue = l.NewValue,
                Operation = l.Operation,
                Source = l.Source,
                RawText = l.RawText,
                Timestamp = l.Timestamp ?? DateTime.UtcNow,
            }).ToList(),
        };

        _db.Households.Add(household);
        await _db.SaveChangesAsync();
        return Ok(new { id = household.Id });
    }

    // استرجاع أسرة بأفرادها وسجل عملياتها
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var household = await _db.Households
            .Include(h => h.Persons)
            .Include(h => h.Logs)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (household is null)
            return NotFound(new { error = "الأسرة غير موجودة" });

        return Ok(household);
    }

    // سجل العمليات لأسرة معيّنة
    [HttpGet("{id:int}/logs")]
    public async Task<IActionResult> Logs(int id)
    {
        var exists = await _db.Households.AnyAsync(h => h.Id == id);
        if (!exists)
            return NotFound(new { error = "الأسرة غير موجودة" });

        var logs = await _db.ActivityLogs
            .Where(l => l.HouseholdId == id)
            .OrderBy(l => l.Timestamp)
            .ToListAsync();
        return Ok(logs);
    }
}
