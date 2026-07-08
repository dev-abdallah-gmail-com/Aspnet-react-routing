// أنواع مشتركة للواجهة الأمامية

// مفاتيح حقول الفرد (تطابق الخادم) مع تسمياتها العربية
export type PersonFieldKey =
  | "name"
  | "birthDate"
  | "gender"
  | "occupation"
  | "nationality"
  | "idNumber"
  | "idExpiryDate"
  | "relationship";

export interface PersonFieldDef {
  key: PersonFieldKey;
  label: string;
  placeholder?: string;
}

// الحقول الثمانية المطلوبة لكل فرد
export const PERSON_FIELDS: PersonFieldDef[] = [
  { key: "name", label: "الاسم", placeholder: "الاسم الكامل" },
  { key: "birthDate", label: "تاريخ الميلاد", placeholder: "مثال: 1990-05-12" },
  { key: "gender", label: "النوع", placeholder: "ذكر / أنثى" },
  { key: "occupation", label: "الوظيفة", placeholder: "المهنة" },
  { key: "nationality", label: "الجنسية", placeholder: "الجنسية" },
  { key: "idNumber", label: "رقم الهوية", placeholder: "رقم الهوية" },
  {
    key: "idExpiryDate",
    label: "تاريخ انتهاء صلاحية الهوية",
    placeholder: "مثال: 2030-01-01",
  },
  { key: "relationship", label: "علاقته بالأسرة", placeholder: "مثال: ابن / زوجة" },
];

export type PersonFields = Partial<Record<PersonFieldKey, string>>;

export interface Person extends PersonFields {
  isHead: boolean;
}

export type LogSource = "voice" | "ocr" | "manual";
export type LogOperation = "add" | "edit" | "raw";

// سطر في شاشة التسجيل (logging)
export interface LogEntry {
  id: string;
  timestamp: string;
  source: LogSource;
  operation: LogOperation;
  personLabel: string;
  field?: PersonFieldKey;
  fieldLabel?: string;
  oldValue?: string;
  newValue?: string;
  rawText?: string;
  message: string;
}

// استجابة نقاط الذكاء الصناعي (voice / ocr)
export interface AiExtractionResponse {
  rawText: string;
  fields: PersonFields;
  error?: string;
}
