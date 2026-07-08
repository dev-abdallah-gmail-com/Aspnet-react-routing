namespace Server.Models;

// جدول واحد يضم رب الأسرة والأفراد معًا، مع عمود IsHead (هل هو رب الأسرة)
public class Person
{
    public int Id { get; set; }

    // الحقول الثمانية المطلوبة لكل فرد
    public string Name { get; set; } = "";
    public string BirthDate { get; set; } = "";
    public string Gender { get; set; } = "";
    public string Occupation { get; set; } = "";
    public string Nationality { get; set; } = "";
    public string IdNumber { get; set; } = "";
    public string IdExpiryDate { get; set; } = "";
    public string Relationship { get; set; } = "";

    // هل هذا الفرد هو رب الأسرة؟ (صف واحد فقط IsHead = true لكل أسرة)
    public bool IsHead { get; set; }

    public int HouseholdId { get; set; }
    public Household? Household { get; set; }
}
