namespace Server.Models;

// سجل عملية إضافة أو تعديل على أي حقل، مصدرها صوتي أو ضوئي أو يدوي
public class ActivityLog
{
    public int Id { get; set; }
    public int HouseholdId { get; set; }
    public Household? Household { get; set; }

    public string PersonLabel { get; set; } = "";   // مثل: رب الأسرة / الفرد 1
    public string FieldName { get; set; } = "";      // مفتاح الحقل
    public string OldValue { get; set; } = "";
    public string NewValue { get; set; } = "";
    public string Operation { get; set; } = "";      // add | edit | raw
    public string Source { get; set; } = "";         // voice | ocr | manual
    public string RawText { get; set; } = "";        // النص الخام المُتعرَّف عليه
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
