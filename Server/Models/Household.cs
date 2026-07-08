namespace Server.Models;

// بيانات الأسرة على مستوى المجموعة: الهاتف وعدد الأفراد + قائمة الأفراد (بمن فيهم رب الأسرة)
public class Household
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = "";
    public int MemberCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Person> Persons { get; set; } = new();
    public List<ActivityLog> Logs { get; set; } = new();
}
