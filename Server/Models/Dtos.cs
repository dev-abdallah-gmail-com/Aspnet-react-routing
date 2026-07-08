namespace Server.Models;

// القيم الحالية للحقول تُرسل من الواجهة ليقرر الذكاء الصناعي التعديل أو الاستبدال
public class PersonFieldsDto
{
    public string? Name { get; set; }
    public string? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? Occupation { get; set; }
    public string? Nationality { get; set; }
    public string? IdNumber { get; set; }
    public string? IdExpiryDate { get; set; }
    public string? Relationship { get; set; }
}

public class VoiceRequest
{
    public string Transcript { get; set; } = "";
    public PersonFieldsDto? CurrentFields { get; set; }
}

public class OcrRequest
{
    public string ImageBase64 { get; set; } = "";
    public string MediaType { get; set; } = "image/jpeg";
    public PersonFieldsDto? CurrentFields { get; set; }
}

// استجابة موحّدة للتعرّف الصوتي والضوئي
public class AiExtractionResponse
{
    public string RawText { get; set; } = "";
    public PersonFieldsDto Fields { get; set; } = new();
    public string? Error { get; set; }
}

// حمولة حفظ الأسرة كاملة
public class SavePersonDto
{
    public string Name { get; set; } = "";
    public string BirthDate { get; set; } = "";
    public string Gender { get; set; } = "";
    public string Occupation { get; set; } = "";
    public string Nationality { get; set; } = "";
    public string IdNumber { get; set; } = "";
    public string IdExpiryDate { get; set; } = "";
    public string Relationship { get; set; } = "";
    public bool IsHead { get; set; }
}

public class SaveLogDto
{
    public string PersonLabel { get; set; } = "";
    public string FieldName { get; set; } = "";
    public string OldValue { get; set; } = "";
    public string NewValue { get; set; } = "";
    public string Operation { get; set; } = "";
    public string Source { get; set; } = "";
    public string RawText { get; set; } = "";
    public DateTime? Timestamp { get; set; }
}

public class SaveHouseholdRequest
{
    public string PhoneNumber { get; set; } = "";
    public int MemberCount { get; set; }
    public List<SavePersonDto> Persons { get; set; } = new();
    public List<SaveLogDto> Logs { get; set; } = new();
}
