import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { saveHousehold } from "../api/client";
import { PERSON_FIELDS } from "../types";
import { personLabel, useCensus } from "../state/CensusContext";

export default function ReviewPage() {
  const { phoneNumber, memberCount, persons, logs, reset } = useCensus();
  const navigate = useNavigate();
  const [saving, setSaving] = useState(false);
  const [result, setResult] = useState<string | null>(null);

  const handleSave = async () => {
    setSaving(true);
    setResult(null);
    try {
      const payload = {
        phoneNumber,
        memberCount,
        persons: persons.map((p) => ({
          name: p.name ?? "",
          birthDate: p.birthDate ?? "",
          gender: p.gender ?? "",
          occupation: p.occupation ?? "",
          nationality: p.nationality ?? "",
          idNumber: p.idNumber ?? "",
          idExpiryDate: p.idExpiryDate ?? "",
          relationship: p.relationship ?? "",
          isHead: p.isHead,
        })),
        logs: logs.map((l) => ({
          personLabel: l.personLabel,
          fieldName: l.field ?? "",
          oldValue: l.oldValue ?? "",
          newValue: l.newValue ?? "",
          operation: l.operation,
          source: l.source,
          rawText: l.rawText ?? "",
          timestamp: l.timestamp,
        })),
      };
      const res = await saveHousehold(payload);
      setResult(`تم حفظ الأسرة بنجاح (رقم ${res.id}).`);
    } catch (e) {
      setResult(`خطأ في الحفظ: ${(e as Error).message}`);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="page">
      <h2>مراجعة وحفظ البيانات</h2>
      <div className="review-summary">
        رقم الهاتف: <strong>{phoneNumber || "—"}</strong> · عدد الأفراد:{" "}
        <strong>{memberCount}</strong>
      </div>

      <div className="review-table-wrap">
        <table className="review-table">
          <thead>
            <tr>
              <th>الفرد</th>
              {PERSON_FIELDS.map((f) => (
                <th key={f.key}>{f.label}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {persons.map((p, i) => (
              <tr key={i}>
                <td>
                  {personLabel(i)}
                  {p.isHead && <span className="badge-head">رب الأسرة</span>}
                </td>
                {PERSON_FIELDS.map((f) => (
                  <td key={f.key}>{p[f.key] || "—"}</td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="page-nav">
        <button type="button" onClick={() => navigate("/head")}>
          → تعديل البيانات
        </button>
        <button
          type="button"
          className="btn-primary"
          onClick={handleSave}
          disabled={saving}
        >
          {saving ? "جارٍ الحفظ…" : "حفظ التعداد"}
        </button>
      </div>

      {result && <div className="save-result">{result}</div>}
      {result?.startsWith("تم") && (
        <button
          type="button"
          className="btn-secondary"
          onClick={() => {
            reset();
            navigate("/");
          }}
        >
          بدء أسرة جديدة
        </button>
      )}
    </div>
  );
}
